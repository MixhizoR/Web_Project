using System.ComponentModel.DataAnnotations;

namespace PropPulse.Models
{
    public class Property
    {
        // ID'si olmalı!
        public int Id { get; set; }  

        // Başlık: Kullanıcı, ilan için bir başlık sağlamalıdır.
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title can be a maximum of 100 characters.")]
        public string Title { get; set; }

        // Fiyat: Fiyat boş olamaz veya negatif olamaz.
        [Required(ErrorMessage = "Price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price cannot be less than 1.")]
        public decimal Price { get; set; }

        // Alan: Emlak alanı 10 ile 10.000 metrekare arasında olmalıdır.
        [Required(ErrorMessage = "Area is required.")]
        [Range(10, 10000, ErrorMessage = "Area must be between 10 and 10,000 square meters.")]
        public int Area { get; set; }

        // Adres: Kullanıcı, ilan için bir adres sağlamalıdır.
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address can be a maximum of 200 characters.")]
        public string Address { get; set; }

       // Açıklama: İsteğe bağlı, en fazla 500 karakter olabilir.
        [StringLength(500, ErrorMessage = "Description can be a maximum of 500 characters.")]
        public string Description { get; set; }

        // Fotoğraflar: Kullanıcı, en az 3 ve en fazla 5 fotoğraf eklemelidir.
        [Required(ErrorMessage = "At least 3 photos are required.")]
        [MinLength(3, ErrorMessage = "You must upload at least 3 photos.")]
        [MaxLength(5, ErrorMessage = "You can upload up to 5 photos.")]
        public List<string> Photos { get; set; }

        // Eşyalı Durum: Kullanıcı, evin eşyalı mı eşyasız mı olduğunu belirtmelidir.
        [Required(ErrorMessage = "You must specify whether the property is furnished.")]
        public bool IsFurnished { get; set; }
    }
}

