using System.ComponentModel.DataAnnotations;

namespace Massage.Models;

public class MassageV    
{
    public int Id { get; set; }

    // Название вида массажа
    [Required]
    public string Name { get; set; }

    // Описание услуги
    public string Description { get; set; }

    // Стоимость
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
}

public class TimeSlot
{
    public int Id { get; set; }

    // Дата/время начала слота
    public DateTime StartDateTime { get; set; }

    // Длительность (например, 60 минут)
    public TimeSpan Duration { get; set; }

    // Занятость слота (true — занят, false — свободен)
    public bool IsBooked { get; set; }
}

public class Booking
{
    public int Id { get; set; }

    // Имя клиента
    [Required]
    public string ClientName { get; set; }

    // Контактный номер телефона
    [Phone]
    public string PhoneNumber { get; set; }

    // Возраст
    [Range(0, 100)]
    public int Age { get; set; }

    // Вес
    [Range(0, 130)]
    public float Weight { get; set; }

    // Дополнительная информация от клиента
    public string Notes { get; set; }

    // Время бронирования
    public DateTime BookingTime { get; set; }

    // Связанный слот времени
    public virtual TimeSlot TimeSlot { get; set; }

    // Выбранный вид массажа
    public virtual Massage SelectedMassage { get; set; }
}