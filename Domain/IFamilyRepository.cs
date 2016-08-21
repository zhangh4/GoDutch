using System.Collections.Generic;

namespace Domain
{
    public interface IFamilyRepository
    {
        IEnumerable<Family> Get();
        Family CreateOrUpdate(Family family);
        void DeleteAll();
    }
}