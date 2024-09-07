using IMS.ApplicationCore.DTO;
using IMS.ApplicationCore.Model;

namespace IMS.ApplicationCore.Interfaces
{
    public interface ILabRepository
    {
        Lab? GetLabEntityById(int id);
        LabDTO? GetLabDTOById(int id);
        List<LabDTO> GetAllLabDTOs();
        bool CheckIfLabExists(string labName, string labCode);
        LabDTO? CreateNewLab(CreateLabDTO createLabDTO);
        LabDTO? UpdateLab(Lab lab, UpdateLabDTO updateLabDTO);
        bool DeleteLab(int id);
    }
}
