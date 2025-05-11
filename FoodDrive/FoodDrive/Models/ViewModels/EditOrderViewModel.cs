using System.ComponentModel.DataAnnotations;

namespace FoodDrive.Models.ViewModels
{
    public class EditOrderViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Оберіть статус")]
        public Status SelectedStatus { get; set; }

        public IEnumerable<Status> AvailableStatuses { get; set; } = Enum.GetValues<Status>();
    }
}
