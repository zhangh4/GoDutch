using System.Collections.Generic;
using GoDutch.Common.Models;

namespace GoDutch.Common.Repository
{
    public interface IFamilyRepository
    {
        IEnumerable<Family> Get();
    }
}