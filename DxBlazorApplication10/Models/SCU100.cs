using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Models;

/// <summary>
/// SC.Users
/// </summary>
[Table("SCU100")]
[Index("Id", Name = "SCU100_AK1", IsUnique = true)]
public partial class SCU100
{
    /// <summary>
    /// 사용자등록ID
    /// </summary>
    [Key]
    [Column("reg_id")]
    public int Reg_Id { get; set; }

    /// <summary>
    /// 아이디
    /// </summary>
    [Column("id")]
    [StringLength(50)]
    public string? Id { get; set; }

    /// <summary>
    /// 암호
    /// </summary>
    [Column("pwd")]
    [StringLength(256)]
    public string? Pwd { get; set; }

    /// <summary>
    /// PDA 사용자암호
    /// </summary>
    [Column("pda_pwd")]
    [StringLength(256)]
    public string? Pda_Pwd { get; set; }

    /// <summary>
    /// 사용자명
    /// </summary>
    [Column("nm")]
    [StringLength(50)]
    public string? Nm { get; set; }

    /// <summary>
    /// 사용자구분 - BC:SC700
    /// </summary>
    [Column("usr_ty")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Usr_Ty { get; set; }

    /// <summary>
    /// 사번
    /// </summary>
    [Column("emp_no")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Emp_No { get; set; }

    /// <summary>
    /// 거래처코드
    /// </summary>
    [Column("cust_cd")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Cust_Cd { get; set; }

    /// <summary>
    /// 언어구분 - BC:SC500
    /// </summary>
    [Column("lan_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Lan_Cd { get; set; }

    /// <summary>
    /// 기본법인코드 - BC:BC110S
    /// </summary>
    [Column("co_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Co_Cd { get; set; }

    /// <summary>
    /// 기본사업장코드 - BC:BC120S
    /// </summary>
    [Column("bs_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Bs_Cd { get; set; }

    /// <summary>
    /// 기본회계사업장코드 - BC:BC150S
    /// </summary>
    [Column("div_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Div_Cd { get; set; }

    /// <summary>
    /// 기본부서코드
    /// </summary>
    [Column("dept_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Dept_Cd { get; set; }

    /// <summary>
    /// 기본생산공장코드
    /// </summary>
    [Column("fac_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Fac_Cd { get; set; }

    /// <summary>
    /// 기본생산작업장코드
    /// </summary>
    [Column("wc_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Wc_Cd { get; set; }

    [Column("wh_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Wh_Cd { get; set; }

    /// <summary>
    /// 기본국가코드 - BC:BC410S
    /// </summary>
    [Column("nat_cd")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Nat_Cd { get; set; }

    /// <summary>
    /// 기본통화구분 - BC:BC400
    /// </summary>
    [Column("cury_bc")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Cury_Bc { get; set; }

    [Column("system_cd")]
    [StringLength(20)]
    [Unicode(false)]
    public string? System_Cd { get; set; }

    /// <summary>
    /// 초기Form여부
    /// </summary>
    [Column("init_yn")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Init_Yn { get; set; }

    /// <summary>
    /// 초기Form코드
    /// </summary>
    [Column("init_frm")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Init_Frm { get; set; }

    /// <summary>
    /// 기본Skin구분
    /// </summary>
    [Column("skin_nm")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Skin_Nm { get; set; }

    /// <summary>
    /// Grid Skin
    /// </summary>
    [Column("grid_skin_nm")]
    [StringLength(50)]
    [Unicode(false)]
    public string? GridSkin_Nm { get; set; }

    /// <summary>
    /// 사용여부 - D:1
    /// </summary>
    [Column("use_yn")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Use_Yn { get; set; }

    /// <summary>
    /// 사용만료일
    /// </summary>
    [Column("end_dt", TypeName = "datetime")]
    public DateTime? End_Dt { get; set; }

    [Column("pwd_yn")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Pwd_Yn { get; set; }

    [Column("pwd_dt", TypeName = "datetime")]
    public DateTime? Pwd_Dt { get; set; }

    [Column("pwd_ty")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Pwd_Ty { get; set; }

    /// <summary>
    /// 입력구분 - BC:SC710
    /// </summary>
    [Column("ent_ty")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Ent_Ty { get; set; }

    /// <summary>
    /// Font Name
    /// </summary>
    [Column("fnt_nm")]
    [StringLength(100)]
    public string? Fnt_Nm { get; set; }

    /// <summary>
    /// Font Size
    /// </summary>
    [Column("fnt_siz", TypeName = "decimal(5, 1)")]
    public decimal? Fnt_Siz { get; set; }

    /// <summary>
    /// 비고
    /// </summary>
    [Column("dsc")]
    [StringLength(50)]
    public string? Dsc { get; set; }

    /// <summary>
    /// 생성자
    /// </summary>
    [Column("cid")]
    public int? Cid { get; set; }

    /// <summary>
    /// 생성일
    /// </summary>
    [Column("cdt", TypeName = "datetime")]
    public DateTime? Cdt { get; set; }

    /// <summary>
    /// 수정자
    /// </summary>
    [Column("mid")]
    public int? Mid { get; set; }

    /// <summary>
    /// 수정일
    /// </summary>
    [Column("mdt", TypeName = "datetime")]
    public DateTime? Mdt { get; set; }

    /// <summary>
    /// 생산인원여부
    /// </summary>
    [Column("prod_yn")]
    [StringLength(1)]
    [Unicode(false)]
    public string? Prod_Yn { get; set; }

    /// <summary>
    /// 비밀번호 재설정 토큰
    /// </summary>
    [Column("ResetToken")]
    [StringLength(256)]
    public string? ResetToken { get; set; }

    /// <summary>
    /// 비밀번호 재설정 만료 시간
    /// </summary>
    [Column("ResetTokenExpires", TypeName = "datetime")]
    public DateTime? ResetTokenExpires { get; set; }
}
