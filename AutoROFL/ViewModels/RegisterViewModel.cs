using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoROFL.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Фамилия")]
        public string FName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Имя")]
        public string SName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Отчество")]
        public string MName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DataType(DataType.Date)]
        [Display(Name = "Год рождения")]
        public DateTime Year { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Адрес проживания")]
        public string Adress { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
