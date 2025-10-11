// BLL/BLLCompras.cs
using System.Collections.Generic;
using MPP;
using BE;

namespace BLL
{
    public class BLLCompras
    {
        private readonly MPPCompras _mpp = new MPPCompras();
        public List<BECompraListItem> ByUser(int userId) => _mpp.ByUser(userId);
    }
}
