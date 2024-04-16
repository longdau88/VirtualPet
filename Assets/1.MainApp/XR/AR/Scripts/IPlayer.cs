using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IPlayer
{
    /// <summary>
    /// Đứng yên
    /// </summary>
    void Idle();
    /// <summary>
    /// Đi bộ
    /// </summary>
    void Walk();
    /// <summary>
    ///Chạy
    /// </summary>
    void Run();
    /// <summary>
    ///Trượt
    /// </summary>
    void Dash();
    /// <summary>
    ///Nhảy
    /// </summary>
    void Jump();
    /// <summary>
    ///Bò trườn
    /// </summary>
    void Crawl();
    /// <summary>
    ///Leo trèo
    /// </summary>
    void Climb();
    /// <summary>
    /// Bay
    /// </summary>
    void Fly();
    /// <summary>
    /// Tạo dáng
    /// </summary>
    void Pose();
    /// <summary>
    /// Tấn công (nói chung)
    /// </summary>
    void Attack();
    /// <summary>
    /// Tấn công bằng tay hoặc vũ phí cầm tay, tầm gần
    /// </summary>
    void Hit();
    /// <summary>
    /// Tấn công bằng vũ khí tầm xa
    /// </summary>
    void Shot();
    /// <summary>
    /// Tấn công bằng chân
    /// </summary>
    void Kick();
    /// <summary>
    /// Ném
    /// </summary>
    void Throw();
    /// <summary>
    /// Sử dụng kỹ năng đặc biệt
    /// </summary>
    void Skill();
    /// <summary>
    /// Cúi người
    /// </summary>
    void Bow();
    /// <summary>
    /// Đu, bám
    /// </summary>
    void Cling();
    /// <summary>
    /// Ngồi
    /// </summary>
    void Sit();
}