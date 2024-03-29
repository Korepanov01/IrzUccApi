﻿using IrzUccApi.Db;
using IrzUccApi.Db.Models;
using IrzUccApi.Enums;
using IrzUccApi.ErrorDescribers;
using IrzUccApi.Models.Dtos;
using IrzUccApi.Models.PagingOptions;
using IrzUccApi.Models.Requests.Position;
using IrzUccApi.Models.Requests.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IrzUccApi.Controllers.Users
{
    [Route("api/positions")]
    [ApiController]
    [Authorize]
    public class PositionsController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public PositionsController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositionsAsync([FromQuery] SearchStringParameters parameters)
        {
            var positionsDtos = await _unitOfWork.Positions.GetDtoListAsync(parameters);

            return Ok(positionsDtos);
        }

        [HttpPost]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> AddPositionAsync([FromBody] AddUpdatePositionRequest request)
        {
            if (await _unitOfWork.Positions.ExistsAsync(request.Name))
                return BadRequest(new[] { RequestErrorDescriber.PositionAlreadyExists });

            var position = new Position
            {
                Name = request.Name
            };

            _unitOfWork.Positions.Add(position);
            await _unitOfWork.SaveAsync();

            return Ok(new PositionDto(position));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> UpdatePositionAsync(Guid id, [FromBody] AddUpdatePositionRequest request)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(id);
            if (position == null)
                return NotFound();

            if (await _unitOfWork.Positions.ExistsAsync(request.Name))
                return BadRequest(new[] { RequestErrorDescriber.PositionAlreadyExists });

            position.Name = request.Name;

            _unitOfWork.Positions.Update(position);
            await _unitOfWork.SaveAsync(); 

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> DeletePositionAsync(Guid id)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(id);
            if (position == null)
                return NotFound();

            if (await _unitOfWork.Positions.OwnedByAnyUserAsync(position))
                return BadRequest(new[] { RequestErrorDescriber.ThereAreUsersWithThisPosition });

            _unitOfWork.Positions.Remove(position);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpPost("add_pos_to_user")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> AddPositionToUserAsync([FromBody] AddPositionToUserRequest request)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(request.PositionId);
            if (position == null)
                return NotFound(new[] { RequestErrorDescriber.PositionDoesntExist });

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            if (await _unitOfWork.Positions.OwnedByUserAsync(position, user))
                return BadRequest(new[] { RequestErrorDescriber.UserAlreadyOnPosition });

            var userPosition = new UserPosition
            {
                Start = request.Start.ToUniversalTime(),
                Position = position,
                User = user
            };
            _unitOfWork.UserPositions.Add(userPosition);
            await _unitOfWork.SaveAsync();

            return Ok();
        }

        [HttpPost("remove_user_position")]
        [Authorize(Roles = RolesNames.Admin)]
        public async Task<IActionResult> RemoveUserPositionAsync([FromBody] RemoveUserPositionRequest request)
        {
            var position = await _unitOfWork.Positions.GetByIdAsync(request.PositionId);
            if (position == null)
                return NotFound(new[] { RequestErrorDescriber.PositionDoesntExist });

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
                return NotFound(new[] { RequestErrorDescriber.UserDoesntExist });

            var userPosition = await _unitOfWork.UserPositions.GetByPositionAndUserAsync(position, user);
            if (userPosition == null)
                return BadRequest(new[] { RequestErrorDescriber.UserIsNotInPosition });

            if (request.End < userPosition.Start)
                return BadRequest(new[] { RequestErrorDescriber.EndTimeIsLessThenStartTime });
            
            userPosition.End = request.End.ToUniversalTime();

            _unitOfWork.UserPositions.Update(userPosition);
            await _unitOfWork.SaveAsync();

            return Ok();
        }
    }
}
