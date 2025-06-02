using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace doan3.Models;

public partial class DacsGplxContext : DbContext
{
    public DacsGplxContext()
    {
    }

    public DacsGplxContext(DbContextOptions<DacsGplxContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BaiThi> BaiThis { get; set; }

    public virtual DbSet<CtDangKyThi> CtDangKyThis { get; set; }

    public virtual DbSet<CtPhieuThanhToan> CtPhieuThanhToans { get; set; }

    public virtual DbSet<DiemDanh> DiemDanhs { get; set; }

    public virtual DbSet<GiaoVien> GiaoViens { get; set; }

    public virtual DbSet<GiayPhepLaiXe> GiayPhepLaiXes { get; set; }

    public virtual DbSet<HangGplx> HangGplxes { get; set; }

    public virtual DbSet<HoSoThiSinh> HoSoThiSinhs { get; set; }

    public virtual DbSet<HocBu> HocBus { get; set; }

    public virtual DbSet<HocVien> HocViens { get; set; }

    public virtual DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; }

    public virtual DbSet<KetQuaThi> KetQuaThis { get; set; }

    public virtual DbSet<KhoaHoc> KhoaHocs { get; set; }

    public virtual DbSet<KyThi> KyThis { get; set; }

    public virtual DbSet<LichHoc> LichHocs { get; set; }

    public virtual DbSet<LichTapLai> LichTapLais { get; set; }

    public virtual DbSet<LichThi> LichThis { get; set; }

    public virtual DbSet<LoaiThi> LoaiThis { get; set; }

    public virtual DbSet<LopHoc> LopHocs { get; set; }

    public virtual DbSet<NguoiGiamSat> NguoiGiamSats { get; set; }

    public virtual DbSet<PhanCongGiamSat> PhanCongGiamSats { get; set; }

    public virtual DbSet<PhanLoaiBaiThi> PhanLoaiBaiThis { get; set; }

    public virtual DbSet<Phanhoi> Phanhois { get; set; }

    public virtual DbSet<QuyDinhHangGplx> QuyDinhHangGplxes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TaiLieu> TaiLieus { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<XeTapLai> XeTapLais { get; set; }

    public virtual DbSet<YeuCauNangHang> YeuCauNangHangs { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=haiit;Initial Catalog=DACS_GPLX;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiThi>(entity =>
        {
            entity.HasKey(e => e.BaithiId).HasName("PK__BAI_THI__BAC1B24DD0C6A423");

            entity.ToTable("BAI_THI");

            entity.HasIndex(e => new { e.KythiId, e.HosoId, e.LoaibaiId }, "UK_BAITHI_KYTHI_HOSO_LOAI").IsUnique();

            entity.Property(e => e.BaithiId).HasColumnName("BAITHI_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.KythiId).HasColumnName("KYTHI_ID");
            entity.Property(e => e.LoaibaiId).HasColumnName("LOAIBAI_ID");
            entity.Property(e => e.Tenbaithi)
                .HasMaxLength(200)
                .HasColumnName("TENBAITHI");
            entity.Property(e => e.Thoigian).HasColumnName("THOIGIAN");
            entity.Property(e => e.Thutu).HasColumnName("THUTU");

            entity.HasOne(d => d.Hoso).WithMany(p => p.BaiThis)
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BAITHI_HOSO");

            entity.HasOne(d => d.Kythi).WithMany(p => p.BaiThis)
                .HasForeignKey(d => d.KythiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BAITHI_KYTHI");

            entity.HasOne(d => d.Loaibai).WithMany(p => p.BaiThis)
                .HasForeignKey(d => d.LoaibaiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BAITHI_PHANLOAI");
        });

        modelBuilder.Entity<CtDangKyThi>(entity =>
        {
            entity.HasKey(e => e.CtDktId).HasName("PK__CT_DANG___81EED661622D0D63");

            entity.ToTable("CT_DANG_KY_THI");

            entity.HasIndex(e => new { e.KythiId, e.HosoId }, "UK_CT_DANG_KY_THI").IsUnique();

            entity.Property(e => e.CtDktId).HasColumnName("CT_DKT_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.KythiId).HasColumnName("KYTHI_ID");
            entity.Property(e => e.LichthiId).HasColumnName("LICHTHI_ID");
            entity.Property(e => e.thanhtoan)
                .HasColumnType("bit")
                .HasColumnName("THANHTOAN");
            entity.Property(e => e.Thoigiandk)
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANDK");
            entity.Property(e => e.Thoigianthi)
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANTHI");

            entity.HasOne(d => d.Hoso).WithMany(p => p.CtDangKyThis)
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDKT_HOSO");

            entity.HasOne(d => d.Kythi).WithMany(p => p.CtDangKyThis)
                .HasForeignKey(d => d.KythiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTDKT_KYTHI");

            entity.HasOne(d => d.Lichthi).WithMany(p => p.CtDangKyThis)
                .HasForeignKey(d => d.LichthiId)
                .HasConstraintName("FK_CT_DANGKY_THI_LICHTHI");
        });

        modelBuilder.Entity<CtPhieuThanhToan>(entity =>
        {
            entity.HasKey(e => e.CtPhieuttId).HasName("PK__CT_PHIEU__386EA5CEA4E635DA");

            entity.ToTable("CT_PHIEU_THANH_TOAN");

            entity.HasIndex(e => new { e.HosoId, e.ThanhtoanId }, "UK_CT_PHIEU_THANH_TOAN").IsUnique();

            entity.Property(e => e.CtPhieuttId).HasColumnName("CT_PHIEUTT_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.Loaiphi)
                .HasMaxLength(100)
                .HasColumnName("LOAIPHI");
            entity.Property(e => e.ThanhtoanId).HasColumnName("THANHTOAN_ID");
            entity.Property(e => e.Thoigianthanhtoan)
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANTHANHTOAN");

            entity.HasOne(d => d.Hoso).WithMany(p => p.CtPhieuThanhToans)
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPHIEUTT_HOSO");

            entity.HasOne(d => d.Thanhtoan).WithMany(p => p.CtPhieuThanhToans)
                .HasForeignKey(d => d.ThanhtoanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPHIEUTT_THANHTOAN");
        });

        modelBuilder.Entity<DiemDanh>(entity =>
        {
            entity.HasKey(e => e.DiemdanhId).HasName("PK__DIEM_DAN__F42BA33D5A3711A0");

            entity.ToTable("DIEM_DANH");

            entity.HasIndex(e => new { e.HosoId, e.LichhocId }, "UK_DIEMDANH_HOSO_LICH").IsUnique();

            entity.Property(e => e.DiemdanhId).HasColumnName("DIEMDANH_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.LichhocId).HasColumnName("LICHHOC_ID");
            entity.Property(e => e.Ngayhoc)
                .HasColumnType("datetime")
                .HasColumnName("NGAYHOC");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(50)
                .HasColumnName("TRANGTHAI");

            entity.HasOne(d => d.Hoso).WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DIEMDANH_HOSO");

            entity.HasOne(d => d.Lichhoc).WithMany(p => p.DiemDanhs)
                .HasForeignKey(d => d.LichhocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DIEMDANH_LICHHOC");
        });

        modelBuilder.Entity<GiaoVien>(entity =>
        {
            entity.HasKey(e => e.GiaovienId).HasName("PK__GIAO_VIE__16D27118AE474DEE");

            entity.ToTable("GIAO_VIEN");

            entity.Property(e => e.GiaovienId).HasColumnName("GIAOVIEN_ID");
            entity.Property(e => e.Chuyenmon)
                .HasMaxLength(100)
                .HasColumnName("CHUYENMON");
            entity.Property(e => e.HangDaotao)
                .HasMaxLength(50)
                .HasColumnName("HANG_DAOTAO");
            entity.Property(e => e.ImgGv).HasColumnName("IMG_GV");
            entity.Property(e => e.Ngaybatdaulamviec).HasColumnName("NGAYBATDAULAMVIEC");
            entity.Property(e => e.Tengiaovien)
                .HasMaxLength(150)
                .HasColumnName("TENGIAOVIEN");
        });

        modelBuilder.Entity<GiayPhepLaiXe>(entity =>
        {
            entity.HasKey(e => e.GplxId).HasName("PK__GIAY_PHE__3C39E18955E2BE7A");

            entity.ToTable("GIAY_PHEP_LAI_XE");

            entity.HasIndex(e => e.HosoId, "UQ__GIAY_PHE__B9E8D37BB96427A5").IsUnique();

            entity.Property(e => e.GplxId).HasColumnName("GPLX_ID");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.Ngaycap).HasColumnName("NGAYCAP");
            entity.Property(e => e.Ngayhethan).HasColumnName("NGAYHETHAN");

            entity.HasOne(d => d.Hang).WithMany(p => p.GiayPhepLaiXes)
                .HasForeignKey(d => d.HangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GPLX_HANG");

            entity.HasOne(d => d.Hoso).WithOne(p => p.GiayPhepLaiXe)
                .HasForeignKey<GiayPhepLaiXe>(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GPLX_HOSO");
        });

        modelBuilder.Entity<HangGplx>(entity =>
        {
            entity.HasKey(e => e.HangId).HasName("PK__HANG_GPL__6B501BD955A2F7FD");

            entity.ToTable("HANG_GPLX");

            entity.HasIndex(e => e.Tenhang, "UQ__HANG_GPL__94F6D962AE584CA7").IsUnique();

            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.Hocvan)
                .HasMaxLength(100)
                .HasColumnName("HOCVAN");
            entity.Property(e => e.Loaiphuongtien)
                .HasMaxLength(100)
                .HasColumnName("LOAIPHUONGTIEN");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.OnlySatHach).HasColumnName("ONLY_SAT_HACH");
            entity.Property(e => e.PhiCapphep)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PHI_CAPPHEP");
            entity.Property(e => e.PhiDaotao)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PHI_DAOTAO");
            entity.Property(e => e.PhiThi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PHI_THI");
            entity.Property(e => e.Suckhoe).HasColumnName("SUCKHOE");
            entity.Property(e => e.Tenhang)
                .HasMaxLength(50)
                .HasColumnName("TENHANG");
            entity.Property(e => e.TgDtLythuyet)
                .HasMaxLength(50)
                .HasColumnName("TG_DT_LYTHUYET");
            entity.Property(e => e.TgDtThuchanh)
                .HasMaxLength(50)
                .HasColumnName("TG_DT_THUCHANH");
            entity.Property(e => e.Tuoitoida).HasColumnName("TUOITOIDA");
            entity.Property(e => e.Tuoitoithieu).HasColumnName("TUOITOITHIEU");
        });

        modelBuilder.Entity<HoSoThiSinh>(entity =>
        {
            entity.HasKey(e => e.HosoId).HasName("PK__HO_SO_TH__B9E8D37AFAE75FF2");

            entity.ToTable("HO_SO_THI_SINH");

            entity.HasIndex(e => new { e.HocvienId, e.HangId }, "UK_HOSO_HOCVIEN_HANG").IsUnique();

            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.HocvienId).HasColumnName("HOCVIEN_ID");
            entity.Property(e => e.ImgThisinh).HasColumnName("IMG_THISINH");
            entity.Property(e => e.Khamsuckhoe)
                .HasMaxLength(100)
                .HasColumnName("KHAMSUCKHOE");
            entity.Property(e => e.LoaiHoso)
                .HasMaxLength(100)
                .HasColumnName("LOAI_HOSO");
            entity.Property(e => e.Ngaydk).HasColumnName("NGAYDK");

            entity.HasOne(d => d.Hang).WithMany(p => p.HoSoThiSinhs)
                .HasForeignKey(d => d.HangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOSO_HANG");

            entity.HasOne(d => d.Hocvien).WithMany(p => p.HoSoThiSinhs)
                .HasForeignKey(d => d.HocvienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOSO_HOCVIEN");
        });

        modelBuilder.Entity<HocBu>(entity =>
        {
            entity.HasKey(e => e.HocbuId).HasName("PK__HOC_BU__7D7D943130FD1471");

            entity.ToTable("HOC_BU");

            entity.Property(e => e.HocbuId).HasColumnName("HOCBU_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.Lephi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("LEPHI");
            entity.Property(e => e.LichhocId).HasColumnName("LICHHOC_ID");
            entity.Property(e => e.Ngayhoc)
                .HasColumnType("datetime")
                .HasColumnName("NGAYHOC");

            entity.HasOne(d => d.Hoso).WithMany(p => p.HocBus)
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOCBU_HOSO");

            entity.HasOne(d => d.Lichhoc).WithMany(p => p.HocBus)
                .HasForeignKey(d => d.LichhocId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOCBU_LICHHOC");
        });

        modelBuilder.Entity<HocVien>(entity =>
        {
            entity.HasKey(e => e.HocvienId).HasName("PK__HOC_VIEN__851EDF58EE81C923");

            entity.ToTable("HOC_VIEN");

            entity.HasIndex(e => e.Socccd, "UQ__HOC_VIEN__01FC44CBE41430A4").IsUnique();

            entity.Property(e => e.HocvienId).HasColumnName("HOCVIEN_ID");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(10)
                .HasColumnName("GIOITINH");
            entity.Property(e => e.Ngaysinh).HasColumnName("NGAYSINH");
            entity.Property(e => e.Socccd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SOCCCD");
            entity.Property(e => e.Tenhocvien)
                .HasMaxLength(150)
                .HasColumnName("TENHOCVIEN");
        });

        modelBuilder.Entity<KetQuaHocTap>(entity =>
        {
            entity.HasKey(e => e.KetquaId).HasName("PK__KET_QUA___11766B1EB5D3C364");

            entity.ToTable("KET_QUA_HOC_TAP");

            // Xóa chỉ mục Unique trên HosoId để cho phép một-nhiều
            // entity.HasIndex(e => e.HosoId, "UK_KQHT_HOSO").IsUnique(); // Xóa dòng này

            entity.Property(e => e.KetquaId).HasColumnName("KETQUA_ID");
            entity.Property(e => e.DauTn)
                .HasMaxLength(50)
                .HasColumnName("DAU_TN");
            entity.Property(e => e.DuDkThisat)
                .HasMaxLength(50)
                .HasColumnName("DU_DK_THISAT");
            entity.Property(e => e.GioBandem).HasColumnName("GIO_BANDEM");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.HtDuongtruong)
                .HasMaxLength(50)
                .HasColumnName("HT_DUONGTRUONG");
            entity.Property(e => e.HtLythuyet)
                .HasMaxLength(50)
                .HasColumnName("HT_LYTHUYET");
            entity.Property(e => e.HtMophong)
                .HasMaxLength(50)
                .HasColumnName("HT_MOPHONG");
            entity.Property(e => e.HtSahinh)
                .HasMaxLength(50)
                .HasColumnName("HT_SAHINH");
            entity.Property(e => e.KmHoanthanh)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("KM_HOANTHANH");
            entity.Property(e => e.LopId).HasColumnName("LOP_ID");
            entity.Property(e => e.Nhanxet).HasColumnName("NHANXET");
            entity.Property(e => e.Sobuoiodahoc).HasColumnName("SOBUOIODAHOC");
            entity.Property(e => e.Sobuoitoithieu).HasColumnName("SOBUOITOITHIEU");
            entity.Property(e => e.Thoigiancapnhat)
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANCAPNHAT");

            entity.HasOne(d => d.Hoso)
                .WithMany(p => p.KetQuaHocTaps) // Sửa thành WithMany và sử dụng KetQuaHocTaps
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KQHT_HOSO");

            entity.HasOne(d => d.Lop)
                .WithMany(p => p.KetQuaHocTaps)
                .HasForeignKey(d => d.LopId)
                .HasConstraintName("FK_KQHT_LOPHOC");
        });

        modelBuilder.Entity<KetQuaThi>(entity =>
        {
            entity.HasKey(e => e.KetquaId).HasName("PK__KET_QUA___11766B1EA7F4B20A");

            entity.ToTable("KET_QUA_THI");

            entity.HasIndex(e => e.BaithiId, "UQ__KET_QUA___BAC1B24CB24E607F").IsUnique();

            entity.Property(e => e.KetquaId).HasColumnName("KETQUA_ID");
            entity.Property(e => e.BaithiId).HasColumnName("BAITHI_ID");
            entity.Property(e => e.Diem)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("DIEM");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.Ketqua)
                .HasMaxLength(50)
                .HasColumnName("KETQUA");

            entity.HasOne(d => d.Baithi).WithOne(p => p.KetQuaThi)
                .HasForeignKey<KetQuaThi>(d => d.BaithiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KQTHI_BAITHI");
        });

        modelBuilder.Entity<KhoaHoc>(entity =>
        {
            entity.HasKey(e => e.KhoahocId).HasName("PK__KHOA_HOC__9E83DE945DBE1C45");

            entity.ToTable("KHOA_HOC");

            entity.Property(e => e.KhoahocId).HasColumnName("KHOAHOC_ID");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Ngaybatdau).HasColumnName("NGAYBATDAU");
            entity.Property(e => e.Ngayketthuc).HasColumnName("NGAYKETTHUC");
            entity.Property(e => e.SlToida).HasColumnName("SL_TOIDA");
            entity.Property(e => e.Tenkhoahoc)
                .HasMaxLength(200)
                .HasColumnName("TENKHOAHOC");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(50)
                .HasColumnName("TRANGTHAI");

            entity.HasOne(d => d.Hang).WithMany(p => p.KhoaHocs)
                .HasForeignKey(d => d.HangId)
                .HasConstraintName("FK_KHOAHOC_HANG");
        });

        modelBuilder.Entity<KyThi>(entity =>
        {
            entity.HasKey(e => e.KythiId).HasName("PK__KY_THI__6C9B16EA38B793C6");

            entity.ToTable("KY_THI");

            entity.Property(e => e.KythiId).HasColumnName("KYTHI_ID");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.LoaithiId).HasColumnName("LOAITHI_ID");
            entity.Property(e => e.Tenkythi)
                .HasMaxLength(150)
                .HasColumnName("TENKYTHI");

            entity.HasOne(d => d.Hang).WithMany(p => p.KyThis)
                .HasForeignKey(d => d.HangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KYTHI_HANG");

            entity.HasOne(d => d.Loaithi).WithMany(p => p.KyThis)
                .HasForeignKey(d => d.LoaithiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KYTHI_LOAITHI");
        });

        modelBuilder.Entity<LichHoc>(entity =>
        {
            entity.HasKey(e => e.LichhocId).HasName("PK__LICH_HOC__EE5812335724675D");

            entity.ToTable("LICH_HOC");

            entity.Property(e => e.LichhocId).HasColumnName("LICHHOC_ID");
            entity.Property(e => e.Hinhthuchoc)
                .HasMaxLength(100)
                .HasColumnName("HINHTHUCHOC");
            entity.Property(e => e.LopId).HasColumnName("LOP_ID");
            entity.Property(e => e.Noidung).HasColumnName("NOIDUNG");
            entity.Property(e => e.TgBatdau)
                .HasColumnType("datetime")
                .HasColumnName("TG_BATDAU");
            entity.Property(e => e.TgKetthuc)
                .HasColumnType("datetime")
                .HasColumnName("TG_KETTHUC");

            entity.HasOne(d => d.Lop).WithMany(p => p.LichHocs)
                .HasForeignKey(d => d.LopId)
                .HasConstraintName("FK_LICHHOC_LOPHOC");
        });

        modelBuilder.Entity<LichTapLai>(entity =>
        {
            entity.HasKey(e => e.LichId).HasName("PK__LICH_TAP__7557C4F1A0EB1782");

            entity.ToTable("LICH_TAP_LAI");

            entity.Property(e => e.LichId).HasColumnName("LICH_ID");
            entity.Property(e => e.Diadiem)
                .HasMaxLength(200)
                .HasColumnName("DIADIEM");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.GiaovienId).HasColumnName("GIAOVIEN_ID");
            entity.Property(e => e.HosoId).HasColumnName("HOSO_ID");
            entity.Property(e => e.Noidung).HasColumnName("NOIDUNG");
            entity.Property(e => e.Tgbatdau)
                .HasColumnType("datetime")
                .HasColumnName("TGBATDAU");
            entity.Property(e => e.Tgketthuc)
                .HasColumnType("datetime")
                .HasColumnName("TGKETTHUC");
            entity.Property(e => e.XeId).HasColumnName("XE_ID");

            entity.HasOne(d => d.Giaovien).WithMany(p => p.LichTapLais)
                .HasForeignKey(d => d.GiaovienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LICHTAPLAI_GIAOVIEN");

            entity.HasOne(d => d.Hoso).WithMany(p => p.LichTapLais)
                .HasForeignKey(d => d.HosoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LICHTAPLAI_HOSO");

            entity.HasOne(d => d.Xe).WithMany(p => p.LichTapLais)
                .HasForeignKey(d => d.XeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LICHTAPLAI_XE");
        });

        modelBuilder.Entity<LichThi>(entity =>
        {
            entity.HasKey(e => e.LichthiId).HasName("PK__LICH_THI__8048B9FD15168F8F");

            entity.ToTable("LICH_THI");

            entity.Property(e => e.LichthiId).HasColumnName("LICHTHI_ID");
            entity.Property(e => e.Diadiemthi)
                .HasMaxLength(200)
                .HasColumnName("DIADIEMTHI");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.Thoigianthi)
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANTHI");
        });

        modelBuilder.Entity<LoaiThi>(entity =>
        {
            entity.HasKey(e => e.LoaithiId).HasName("PK__LOAI_THI__9F7F80D8D6D728C4");

            entity.ToTable("LOAI_THI");

            entity.HasIndex(e => e.Tenloaithi, "UQ__LOAI_THI__BEC1476062A03B27").IsUnique();

            entity.Property(e => e.LoaithiId).HasColumnName("LOAITHI_ID");
            entity.Property(e => e.Tenloaithi)
                .HasMaxLength(100)
                .HasColumnName("TENLOAITHI");
        });

        modelBuilder.Entity<LopHoc>(entity =>
        {
            entity.HasKey(e => e.LopId).HasName("PK__LOP_HOC__1EE1BC50A37C894B");

            entity.ToTable("LOP_HOC");

            entity.Property(e => e.LopId).HasColumnName("LOP_ID");
            entity.Property(e => e.GiaovienId).HasColumnName("GIAOVIEN_ID");
            entity.Property(e => e.KhoahocId).HasColumnName("KHOAHOC_ID");
            entity.Property(e => e.LoaiLop)
                .HasMaxLength(50)
                .HasColumnName("LOAI_LOP");
            entity.Property(e => e.Tenlop)
                .HasMaxLength(100)
                .HasColumnName("TENLOP");

            entity.HasOne(d => d.Giaovien).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.GiaovienId)
                .HasConstraintName("FK_LOPHOC_GIAOVIEN");

            entity.HasOne(d => d.Khoahoc).WithMany(p => p.LopHocs)
                .HasForeignKey(d => d.KhoahocId)
                .HasConstraintName("FK_LOPHOC_KHOAHOC");
        });

        modelBuilder.Entity<NguoiGiamSat>(entity =>
        {
            entity.HasKey(e => e.NguoigsId).HasName("PK__NGUOI_GI__D0BCCAD035566BD0");

            entity.ToTable("NGUOI_GIAM_SAT");

            entity.HasIndex(e => e.Sdt, "UK_NGUOIGS_SDT").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NGUOI_GI__161CF7241DA4FFE8").IsUnique();

            entity.Property(e => e.NguoigsId).HasColumnName("NGUOIGS_ID");
            entity.Property(e => e.Donvi)
                .HasMaxLength(100)
                .HasColumnName("DONVI");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Hoten)
                .HasMaxLength(150)
                .HasColumnName("HOTEN");
            entity.Property(e => e.NhomNguoigs)
                .HasMaxLength(100)
                .HasColumnName("NHOM_NGUOIGS");
            entity.Property(e => e.Sdt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<PhanCongGiamSat>(entity =>
        {
            entity.HasKey(e => e.PhancongGsId).HasName("PK__PHAN_CON__87B1AADF59E9C4CD");

            entity.ToTable("PHAN_CONG_GIAM_SAT");

            entity.HasIndex(e => new { e.NguoigsId, e.BaithiId }, "UK_PHAN_CONG_GIAM_SAT").IsUnique();

            entity.Property(e => e.PhancongGsId).HasColumnName("PHANCONG_GS_ID");
            entity.Property(e => e.BaithiId).HasColumnName("BAITHI_ID");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.NguoigsId).HasColumnName("NGUOIGS_ID");
            entity.Property(e => e.Vaitro)
                .HasMaxLength(100)
                .HasColumnName("VAITRO");

            entity.HasOne(d => d.Baithi).WithMany(p => p.PhanCongGiamSats)
                .HasForeignKey(d => d.BaithiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHANCONGGS_BAITHI");

            entity.HasOne(d => d.Nguoigs).WithMany(p => p.PhanCongGiamSats)
                .HasForeignKey(d => d.NguoigsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHANCONGGS_NGUOIGS");
        });

        modelBuilder.Entity<PhanLoaiBaiThi>(entity =>
        {
            entity.HasKey(e => e.LoaibaiId).HasName("PK__PHAN_LOA__C23CDC982D6EA04C");

            entity.ToTable("PHAN_LOAI_BAI_THI");

            entity.HasIndex(e => e.Tenloaibai, "UQ__PHAN_LOA__804A6788FDE16D88").IsUnique();

            entity.Property(e => e.LoaibaiId).HasColumnName("LOAIBAI_ID");
            entity.Property(e => e.Mota).HasColumnName("MOTA");
            entity.Property(e => e.Tenloaibai)
                .HasMaxLength(100)
                .HasColumnName("TENLOAIBAI");
        });

        modelBuilder.Entity<Phanhoi>(entity =>
        {
            entity.HasKey(e => e.PhanhoiId).HasName("PK__PHANHOI__0D24719199663C58");

            entity.ToTable("PHANHOI");

            entity.Property(e => e.PhanhoiId).HasColumnName("PHANHOI_ID");
            entity.Property(e => e.HocvienId).HasColumnName("HOCVIEN_ID");
            entity.Property(e => e.Noidung).HasColumnName("NOIDUNG");
            entity.Property(e => e.Thoigianph)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANPH");

            entity.HasOne(d => d.Hocvien).WithMany(p => p.Phanhois)
                .HasForeignKey(d => d.HocvienId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHANHOI_HOCVIEN");
        });

        modelBuilder.Entity<QuyDinhHangGplx>(entity =>
        {
            entity.HasKey(e => e.QuydinhId).HasName("PK__QUY_DINH__E82A2086F9CFD6DE");

            entity.ToTable("QUY_DINH_HANG_GPLX");

            entity.Property(e => e.QuydinhId).HasColumnName("QUYDINH_ID");
            entity.Property(e => e.DuongTruong).HasColumnName("DUONG_TRUONG");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.KmToithieu).HasColumnName("KM_TOITHIEU");
            entity.Property(e => e.LyThuyet).HasColumnName("LY_THUYET");
            entity.Property(e => e.MoPhong).HasColumnName("MO_PHONG");
            entity.Property(e => e.SaHinh).HasColumnName("SA_HINH");
            entity.Property(e => e.SogioBandem).HasColumnName("SOGIO_BANDEM");

            entity.HasOne(d => d.Hang).WithMany(p => p.QuyDinhHangGplxes)
                .HasForeignKey(d => d.HangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QUYDINHHANG_HANG");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK___ROLE__E5B2D4DB848981E6");

            entity.ToTable("_ROLE");

            entity.HasIndex(e => e.Rolename, "UK_ROLE_ROLENAME").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("_ROLE_ID");
            entity.Property(e => e.Description).HasColumnName("_DESCRIPTION");
            entity.Property(e => e.Rolename)
                .HasMaxLength(100)
                .HasColumnName("_ROLENAME");
        });

        modelBuilder.Entity<TaiLieu>(entity =>
        {
            entity.HasKey(e => e.TailieuId).HasName("PK__TAI_LIEU__4997D23EF5B7560F");

            entity.ToTable("TAI_LIEU");

            entity.Property(e => e.TailieuId).HasColumnName("TAILIEU_ID");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.Sl).HasColumnName("SL");
            entity.Property(e => e.Tentl)
                .HasMaxLength(200)
                .HasColumnName("TENTL");
            entity.Property(e => e.Thoigiancapnhat)
                .HasColumnType("datetime")
                .HasColumnName("THOIGIANCAPNHAT");

            entity.HasOne(d => d.Hang).WithMany(p => p.TaiLieus)
                .HasForeignKey(d => d.HangId)
                .HasConstraintName("FK_TAILIEU_HANG");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.ThanhtoanId).HasName("PK__THANH_TO__5C1FA3DAC18DE288");

            entity.ToTable("THANH_TOAN");

            entity.Property(e => e.ThanhtoanId).HasColumnName("THANHTOAN_ID");
            entity.Property(e => e.Ghichu).HasColumnName("GHICHU");
            entity.Property(e => e.Phuongthuc)
                .HasMaxLength(100)
                .HasColumnName("PHUONGTHUC");
            entity.Property(e => e.Sotien)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SOTIEN");
            entity.Property(e => e.TenThanhToan)
                .HasMaxLength(150)
                .HasColumnName("TEN_THANH_TOAN");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(50)
                .HasColumnName("TRANGTHAI");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK___USER__B1E72F8E068FF0F9");

            entity.ToTable("_USER");

            entity.HasIndex(e => e.Sdt, "UQ___USER__067314A57E258952").IsUnique();

            entity.HasIndex(e => e.Email, "UQ___USER__3DFF6ACF4EB1A133").IsUnique();

            entity.HasIndex(e => e.Username, "UQ___USER__B45346BB921DC585").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("_USER_ID");
            entity.Property(e => e.Createat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("_CREATEAT");
            entity.Property(e => e.Diachi)
                .HasMaxLength(100)
                .HasColumnName("_DIACHI");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("_EMAIL");
            entity.Property(e => e.Isactive)
                .HasDefaultValue(true)
                .HasColumnName("_ISACTIVE");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("_PASSWORD");
            entity.Property(e => e.Referenceld).HasColumnName("_REFERENCELD");
            entity.Property(e => e.RoleId).HasColumnName("_ROLE_ID");
            entity.Property(e => e.Sdt)
                .HasMaxLength(100)
                .HasColumnName("_SDT");
            entity.Property(e => e.Updateat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("_UPDATEAT");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("_USERNAME");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_USER_ROLE");
        });

        modelBuilder.Entity<XeTapLai>(entity =>
        {
            entity.HasKey(e => e.XeId).HasName("PK__XE_TAP_L__BC2BDA734D869450");

            entity.ToTable("XE_TAP_LAI");

            entity.Property(e => e.XeId).HasColumnName("XE_ID");
            entity.Property(e => e.Loaixe)
                .HasMaxLength(100)
                .HasColumnName("LOAIXE");
            entity.Property(e => e.Trangthai)
                .HasMaxLength(50)
                .HasColumnName("TRANGTHAI");
        });

        modelBuilder.Entity<YeuCauNangHang>(entity =>
        {
            entity.HasKey(e => e.YeucauId).HasName("PK__YEU_CAU___1EBF362A91F35462");

            entity.ToTable("YEU_CAU_NANG_HANG");

            entity.Property(e => e.YeucauId).HasColumnName("YEUCAU_ID");
            entity.Property(e => e.HangId).HasColumnName("HANG_ID");
            entity.Property(e => e.SonamKn).HasColumnName("SONAM_KN");
            entity.Property(e => e.Yeucau).HasColumnName("YEUCAU");

            entity.HasOne(d => d.Hang).WithMany(p => p.YeuCauNangHangs)
                .HasForeignKey(d => d.HangId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_YEUCAU_HANG");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
