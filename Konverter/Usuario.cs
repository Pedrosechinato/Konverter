using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konverter
{
    class Usuario
    {
        private string id;
        private string nome;
        private string senha;
        private int perfil;
        private DateTime alteracaoSenha;
        private string rg;
        private string status;

        public Usuario(string id, string nome, string senha, int perfil, DateTime alteracaoSenha, string rg, string status)
        {
            this.Id = id;
            this.Nome = nome;
            this.Senha = senha;
            this.Perfil = perfil;
            this.AlteracaoSenha = alteracaoSenha;
            this.Rg = rg;
            this.Status = status;
        }

        public string Id { get => id; set => id = value; }
        public string Nome { get => nome; set => nome = value; }
        public string Senha { get => senha; set => senha = value; }
        public int Perfil { get => perfil; set => perfil = value; }
        public DateTime AlteracaoSenha { get => alteracaoSenha; set => alteracaoSenha = value; }
        public string Rg { get => rg; set => rg = value; }
        public string Status { get => status; set => status = value; }

        public override string ToString()
        {
            return Id +
                    " " + Nome +
                    " " + senha +
                    " " + Perfil +
                    " " + Rg +
                    " " + Status;
        }
    }
}
