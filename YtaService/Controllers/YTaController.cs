using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YtaService.BLL.Interfaces;
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho tất cả endpoints
    public class YtaController : ControllerBase
    {
        private readonly IYtaBusiness _business;

        public YtaController(IYtaBusiness business)
        {
            _business = business;
        }

        // 1. Search
        [Route("tim-kiem")]
        /// <summary>
        /// Tìm kiếm Y tá
        /// Quyền: Admin, YTa
        /// </summary>
        [Route("search")]
        [HttpPost]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult Search([FromBody] YTaSearchDTO model)
        {
            try
            {
                long total = 0;
                var data = _business.Search(model, out total);

                var viewData = data.Select(x => new YTaViewDTO
                {
                    Id = x.Id,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    GioiTinh = x.GioiTinh,
                    SoDienThoai = x.SoDienThoai,
                    KhoaId = x.KhoaId,
                    ChungChiHanhNghe = x.ChungChiHanhNghe
                }).ToList();

                var result = new PagedResult<YTaViewDTO>
                {
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize,
                    TotalRecords = total,
                    Items = viewData
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. Create
        [Route("tao-moi")]
        /// <summary>
        /// Thêm Y tá mới
        /// Quyền: Chỉ Admin
        /// </summary>
        [Route("create")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] YtaCreateDTO model)
        {
            try
            {
                var entity = new YTa
                {
                    Id = Guid.NewGuid(),
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    SoDienThoai = model.SoDienThoai,
                    KhoaId = model.KhoaId,
                    ChungChiHanhNghe = model.ChungChiHanhNghe
                };

                if (_business.Create(entity))
                {
                    return Ok(new { Message = "Thêm Y tá thành công", Data = entity });
                }
                return BadRequest("Thêm thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 6. Get All
        [Route("danh-sach")]
        /// <summary>
        /// Lấy tất cả Y tá
        /// Quyền: Admin, YTa
        /// </summary>
        [Route("get-all")]
        [HttpGet]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _business.GetAll();

                var viewData = data.Select(x => new YTaViewDTO
                {
                    Id = x.Id,
                    HoTen = x.HoTen,
                    NgaySinh = x.NgaySinh,
                    GioiTinh = x.GioiTinh,
                    SoDienThoai = x.SoDienThoai,
                    KhoaId = x.KhoaId,
                    ChungChiHanhNghe = x.ChungChiHanhNghe
                }).ToList();

                return Ok(viewData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 3. Update
        [Route("cap-nhat")]
        /// <summary>
        /// Cập nhật Y tá
        /// Quyền: Admin hoặc YTa (sửa thông tin của mình)
        /// </summary>
        [Route("update")]
        [HttpPut]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult Update([FromBody] YTaUpdateDTO model)
        {
            try
            {
                var entity = new YTa
                {
                    Id = model.Id,
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh,
                    GioiTinh = model.GioiTinh,
                    SoDienThoai = model.SoDienThoai,
                    KhoaId = model.KhoaId,
                    ChungChiHanhNghe = model.ChungChiHanhNghe
                };

                if (_business.Update(entity))
                {
                    return Ok(new { Message = "Cập nhật thành công" });
                }
                return BadRequest("Cập nhật thất bại hoặc không tìm thấy ID");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 4. Delete
        [Route("xoa/{id}")]
        /// <summary>
        /// Xóa Y tá
        /// Quyền: Chỉ Admin
        /// </summary>
        [Route("delete/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            if (_business.Delete(id)) return Ok(new { Message = "Xóa thành công" });
            return BadRequest("Xóa thất bại");
        }

        // 5. GetById
        [Route("chi-tiet/{id}")]
        /// <summary>
        /// Lấy Y tá theo ID
        /// Quyền: Admin, YTa
        /// </summary>
        [Route("get-by-id/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin,YTa")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);
            if (data == null) return NotFound("Không tìm thấy");

            var viewDto = new YTaViewDTO
            {
                Id = data.Id,
                HoTen = data.HoTen,
                NgaySinh = data.NgaySinh,
                GioiTinh = data.GioiTinh,
                SoDienThoai = data.SoDienThoai,
                KhoaId = data.KhoaId,
                ChungChiHanhNghe = data.ChungChiHanhNghe
            };
            return Ok(viewDto);
        }
    }
}