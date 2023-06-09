﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ar_dashboard.Models;
using ar_dashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ar_dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Asset3DController : ControllerBase
    {
        private readonly IUserDbService _userDbService;
        private readonly DatabaseController _databaseController;
        private readonly IConfiguration _configuration;
        private readonly CacheController _cacheController;
        public Asset3DController(DatabaseController databaseController, IConfiguration configuration, CacheController cacheController)
        {
            _userDbService = databaseController.UserDbService ?? throw new ArgumentNullException(nameof(databaseController));
            _databaseController = databaseController;
            _configuration = configuration;
            _cacheController = cacheController;
        }

        [Authorize]
        [HttpPost]
        [RequestSizeLimit(737280000)]
        public async Task<IActionResult> CreateAsset3D(IFormFile file)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // after load success to storage

                // get from cache first
                var userData = _cacheController.GetUserData(userId);

                if (userData == null) // if data is not saved in cache, get from db
                {
                    userData = await _userDbService.GetAsync(userId);
                }
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }



                if (file.Length > 0)
                {
                    var assetId = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file.FileName);
                    var assetName = Path.GetFileNameWithoutExtension(file.FileName);

                    if (extension != ".glb")
                    {
                        return BadRequest("model 3d not in valid format (only support glb), current is " + extension);
                    }


                    Asset3D asset = new Asset3D();
                    var filesController = new FilesController(_databaseController, _configuration);


                    var upload = await filesController.processLoadFile(userId, file);

                    if (upload == null)
                    {
                        return BadRequest("can not load file to storage");
                    }

                    asset.Url = upload.Url;
                    asset.Id = assetId;
                    asset.Name = assetName;

                    // save data to db
                    userData.Assets.Add(asset);

                    await _userDbService.UpdateAsync(userId, userData);
                    _cacheController.SetUserData(userId, userData); // save to cache


                    return Ok(new { asset });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }


        [HttpGet("{userId}/{fileName}")]
        public async Task<IActionResult> downloadFile(string userId, string fileName)
        {
            try
            {
                var folderName = Path.Combine("Resources/Files", userId);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var filepath = Path.Combine(pathToSave, fileName);

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filepath, out var contentType))
                {
                    contentType = "application/octest-stream";
                }

                var bytes = await System.IO.File.ReadAllBytesAsync(filepath);
                return File(bytes, contentType, Path.GetFileName(filepath));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> RemoveAll()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // after load success to storage

                // get from cache first
                var userData = _cacheController.GetUserData(userId);

                if (userData == null) // if data is not saved in cache, get from db
                {
                    userData = await _userDbService.GetAsync(userId);
                }

                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                userData.Assets.Clear();
                await _userDbService.UpdateAsync(userId, userData);
                _cacheController.SetUserData(userId, userData); // save to cache

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }

        [HttpDelete("{assetId}")]
        [Authorize]
        public async Task<IActionResult> RemoveModel(string assetId)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                IList<Claim> claim = identity.Claims.ToList();
                var userId = claim[0].Value;

                // after load success to storage
                var userData = await _userDbService.GetAsync(userId);
                if (userData == null)
                {
                    return BadRequest("user data is null");
                }

                var idx = userData.Assets.FindIndex(a =>a.Id == assetId
                );
                if (idx == -1) return BadRequest("not found");
                userData.Assets.RemoveAt(idx);
                await _userDbService.UpdateAsync(userId, userData);
                _cacheController.SetUserData(userId, userData); // save to cache
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internel server error: {e}");
            }
        }
    }
}
