using System;
using System.ComponentModel.DataAnnotations;

namespace Dev.API.Infras
{
    public class RefreshToken
    {
        /// <summary>
        /// jwtID
        /// </summary>
        [Required]
        public string? JwtId { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        [Required]
        public string? Token { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        [Required]
        public DateTime ExpiredAt { get; set; }
    }
}
