using Abstracciones;
using BE;
using MPP;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLLBitacora
    {
        private readonly MPPBitacora _mpp;

        public BLLBitacora()
        {
            _mpp = new MPPBitacora();
        }
        public void Registrar(int? userId, string descripcion, string agente)
        {
            _mpp.Insertar(userId, descripcion, agente);
        }
        

        public List<BE.BEBitacora> Buscar(BE.BEBitacoraFiltro filtro)
        {
           return  _mpp.Buscar(filtro);
        }
    }
}
