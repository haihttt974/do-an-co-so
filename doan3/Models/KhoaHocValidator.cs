using System;
using System.ComponentModel.DataAnnotations;
using doan3.Models;

public static class KhoaHocValidator
{
    public static ValidationResult? ValidateNgayBatDau(DateOnly ngayBatDau, ValidationContext context)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return ngayBatDau > today
            ? ValidationResult.Success
            : new ValidationResult("Ngày bắt đầu phải sau hôm nay.");
    }

    public static ValidationResult? ValidateNgayKetThuc(DateOnly ngayKetThuc, ValidationContext context)
    {
        var instance = context.ObjectInstance as KhoaHoc;
        if (instance == null) return ValidationResult.Success;

        return ngayKetThuc > instance.Ngaybatdau
            ? ValidationResult.Success
            : new ValidationResult("Ngày kết thúc phải sau ngày bắt đầu.");
    }
}