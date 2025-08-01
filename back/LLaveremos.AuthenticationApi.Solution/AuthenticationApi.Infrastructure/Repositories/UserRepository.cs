﻿using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using Llaveremos.SharedLibrary.Responses;
using AuthenticationApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthenticationApi.Infrastructure.Repositories;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationApi.Infrastructure.Repositories
{
    internal class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        public async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user is null ? null! : user!;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user is not null ? new GetUserDTO(
                user.Id, user.Name!, user.TelephoneNumber!, user.Adress!, user.Email!, user.Role!
                ) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null)
                return new Response(false, "invalid credentials");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (!verifyPassword)
                return new Response(false, "invalid credentials");

            string token = GenerateToken(getUser);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Name!),
        new(ClaimTypes.Email, user.Email!),
        new("UserId", user.Id.ToString()) // Agrega el userId como claim
    };

            if (!string.IsNullOrEmpty(user.Role) && !Equals("string", user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Ajusta la expiración según sea necesario
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getUser = await GetUserByEmail(appUserDTO.Email);

            if (getUser is not null)
                return new Response(false, $"invalid email for registration");

            var result = context.Users.Add(new AppUser()
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Adress = appUserDTO.Address,
                Role = appUserDTO.Role
            });

            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "User registered") : 
                new Response(false, "Invalid data");  
        }

        public async Task<IEnumerable<GetUserDTO>> GetAllUsers()
        {
            var users = await context.Users.ToListAsync();
            return users.Select(user => new GetUserDTO(
                    user.Id, user.Name!, user.TelephoneNumber!, user.Adress!, user.Email!, user.Role!
                ));
        }

        public async Task<Response> EditUserById(AppUserDTO appUserDTO)
        {
            var user = await context.Users.FindAsync(appUserDTO.Id);
            if (user is null)
                return new Response(false, "User not found");

            user.Name = appUserDTO.Name;
            user.TelephoneNumber = appUserDTO.TelephoneNumber;
            user.Adress = appUserDTO.Address;
            user.Email = appUserDTO.Email;
            user.Role = appUserDTO.Role;
            if (!string.IsNullOrEmpty(appUserDTO.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password);
            }

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return new Response(true, "User updated successfully");
        }
    }
}
