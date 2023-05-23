﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using IoTHubAPI.Models;
using IoTHubAPI.Models.Dtos;
using IoTHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoTHubAPI.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), 500)]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _usersRepo;
        public UsersController(IUserRepository usersRepo) {
            _usersRepo = usersRepo;
        }

        [ProducesResponseType(typeof(IEnumerable<User>), (int)HttpStatusCode.OK)]
        [HttpGet(Name = "GetAllUsers")]
        public async Task<IActionResult> GetUsers() {
            IEnumerable<User> users = await _usersRepo.GetAllUsers();

            //var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(users);
        }

        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [HttpGet("{userId:length(24)}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string userId) {
            var user = await _usersRepo.GetUser(userId);

            //var userToReturn = _mapper.Map<UserForListDto>(user);

            if(user == null) {
                return NoContent();
            }

            return Ok(user);
        }

        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [HttpGet("GetUserByEmail", Name = "GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromBody] string email)
        {
            var user = await _usersRepo.GetUserByEmail(email);

            //var userToReturn = _mapper.Map<UserForListDto>(user);

            if (user == null)
            {
                return NoContent();
            }

            return Ok(user);
        }
    }
}
