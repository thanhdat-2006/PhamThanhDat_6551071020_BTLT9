using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuApp.Models;

namespace QuanLyChiTieuApp.Services;

public class ChiTieuService
{
    public async Task<List<ChiTieu>> GetAllAsync()
    {
        using var context = new QlChiTieuContext();
        return await context.ChiTieus
            .OrderByDescending(c => c.NgayChi)
            .ThenByDescending(c => c.MaCt)
            .ToListAsync();
    }

    public async Task<List<ChiTieu>> FilterAsync(string? keyword, string? loaiChi, DateOnly? fromDate, DateOnly? toDate)
    {
        using var context = new QlChiTieuContext();
        var query = context.ChiTieus.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(c => c.NoiDung.ToLower().Contains(kw) || 
                                     (c.GhiChu != null && c.GhiChu.ToLower().Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(loaiChi) && loaiChi != "Tất cả")
        {
            query = query.Where(c => c.LoaiChi == loaiChi);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(c => c.NgayChi >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(c => c.NgayChi <= toDate.Value);
        }

        return await query
            .OrderByDescending(c => c.NgayChi)
            .ThenByDescending(c => c.MaCt)
            .ToListAsync();
    }

    public async Task<ChiTieu> AddAsync(ChiTieu chiTieu)
    {
        using var context = new QlChiTieuContext();
        context.ChiTieus.Add(chiTieu);
        await context.SaveChangesAsync();
        return chiTieu;
    }

    public async Task<bool> UpdateAsync(ChiTieu chiTieu)
    {
        using var context = new QlChiTieuContext();
        var existing = await context.ChiTieus.FindAsync(chiTieu.MaCt);
        if (existing == null) return false;

        existing.NgayChi = chiTieu.NgayChi;
        existing.NoiDung = chiTieu.NoiDung;
        existing.LoaiChi = chiTieu.LoaiChi;
        existing.SoTien = chiTieu.SoTien;
        existing.GhiChu = chiTieu.GhiChu;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int maCt)
    {
        using var context = new QlChiTieuContext();
        var existing = await context.ChiTieus.FindAsync(maCt);
        if (existing == null) return false;

        context.ChiTieus.Remove(existing);
        await context.SaveChangesAsync();
        return true;
    }
}
