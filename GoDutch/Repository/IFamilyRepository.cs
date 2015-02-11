using System.Collections.Generic;
using GoDutch.Models;

namespace GoDutch.Repository
{
    public interface IFamilyRepository
    {
        IEnumerable<Family> Get();
    }
}