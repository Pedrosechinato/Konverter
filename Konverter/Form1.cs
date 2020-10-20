using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Konverter
{
    public partial class FormPrincipal : Form
    {
        private int tabAnt;
        Usuario usuarioSessao;
        char[] cifra = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' ','0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        String textoCriptografado;
        BindingList<Usuario> usuarios = new BindingList<Usuario>();

        public FormPrincipal()
        {
            InitializeComponent();
            tabCtrlTopo.Controls.Remove(tabCadas);
            tabCtrlTopo.Controls.Remove(tabData);
            tabCtrlTopo.Controls.Remove(tabBloqueados);
            tabCtrlTopo.Controls.Remove(tabEscalas);



        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Usuario novoUsuario = new Usuario("0001", "admin", "admin", 1, DateTime.Now.AddDays(-150), "12312312", "Normal");
            usuarios.Add(novoUsuario);
            novoUsuario = new Usuario("0002", "Rodolfo Sobrenome", "admin", 2, DateTime.Now.AddDays(-181), "12312312", "Normal");
            usuarios.Add(novoUsuario);
            novoUsuario = new Usuario("0003", "Jose Sobrenome", "admin", 3, DateTime.Now.AddDays(-1), "12312312", "Senha Inicial");
            usuarios.Add(novoUsuario);
            novoUsuario = new Usuario("0004", "Maria Sobrenome", "admin", 4, DateTime.Now.AddDays(-1), "12312312", "Senha Inicial");
            usuarios.Add(novoUsuario);
            novoUsuario = new Usuario("0005", "Zucreide Sobrenome", "admin", 1, DateTime.Now.AddDays(-1), "12312312", "Bloqueado");
            usuarios.Add(novoUsuario);

            dgvUsuarios.DataSource = usuarios;


            populaBloqueados();

        }

        private void tbSenha_teste_KeyUp(object sender, KeyEventArgs e)
        {
            string texto = tbSenha_teste.Text;

            try
            {    //Restriçoes para a nova senha

                //Limpa as check boxes quando o campo está vazio            
                if (texto == "")
                {
                    normal(checkBox1);
                    normal(checkBox2);
                    normal(checkBox3);
                    normal(checkBox4);
                    normal(checkBox5);
                    normal(checkBox6);
                    label17.Text = "";
                    return;
                }

                //Tamanho da senha deve estar entre 7 e 11      
                if (texto.Length >= 7 && texto.Length <= 11)
                {
                    CheckVerde(checkBox1);
                }
                else
                {
                    CheckVermelho(checkBox1);
                }

                //Não pode conter caracteres especiais,espaço ou sombolos matematicos
                if ((texto.All(c => char.IsLetter(c) || char.IsDigit(c))))
                {
                    CheckVerde(checkBox2);
                }
                else
                {
                    CheckVermelho(checkBox2);
                }

                int letras = 0, numeros = 0;
                foreach (char s in texto)
                {
                    if (Char.IsDigit(s))
                    {
                        numeros++;

                    }
                    if (Char.IsLetter(s))
                    {
                        letras++;
                    }
                }

                //A senha deve Conter no mínimo 3 letras
                if (letras >= 3)
                {
                    CheckVerde(checkBox3);
                }
                else
                {
                    CheckVermelho(checkBox3);
                }

                //A senha deve Conter no mínimo 2 numeros
                if (numeros >= 2)
                {
                    CheckVerde(checkBox4);
                }
                else
                {
                    CheckVermelho(checkBox4);
                }

                //comapara com a senha atual
                if (usuarioSessao.Senha != texto)
                {
                    CheckVerde(checkBox5);
                }
                else
                {
                    CheckVermelho(checkBox5);
                }

                if (texto.Length > 2)  // Verificando se o vetor tem condição de fazer a comparação
                {
                    for (int i = 0; i < texto.Length; i++)
                    {
                        if (!texto.Contains(texto[i].ToString() + texto[i].ToString() + texto[i].ToString()))
                        {
                            CheckVerde(checkBox6);
                        }
                        else
                        {
                            CheckVermelho(checkBox6);
                            break;
                        }
                    }
                }
                else {
                    normal(checkBox6);
                }

                //verifica se todos os checkboxes estao checados e abilita o botão de trocar senha
                if (checkBox1.Checked && checkBox2.Checked &&
                    checkBox3.Checked && checkBox4.Checked &&
                    checkBox5.Checked && checkBox6.Checked)
                {
                    BUTvalida.Enabled = true;

                    //Verificação de força da senha 

                    int nota = 10;

                    //Se tem só 3 letras
                    if (letras == 3) { nota--; }

                    //Se tem só 2 numeros
                    if (numeros == 2) { nota--; }

                    //Separa os nomes
                    var nomes = usuarioSessao.Nome.Split(' ');

                    //Se tem o primeiro nome na senha
                    if (texto.Contains(nomes[0]))
                    {
                        nota -= 2;
                    }

                    //Se tem as iniciais
                    string iniciais = "";
                    for (int i = 0; i < nomes.Length; i++)
                    {
                        iniciais += nomes[i][0];

                    }
                    if (texto.Contains(iniciais)) {
                        nota -= 3;
                    }

                    //Se tem sequência crescente
                    nota -= Sequencia(texto);

                    //Se tem sequência decrescente
                    char[] charArray = texto.ToCharArray();
                    Array.Reverse(charArray);
                    nota -= Sequencia(new string(charArray));

                    //Se tem os 4 primeiros numero do ID do usuario
                    if (texto.Contains(usuarioSessao.Id.Substring(0, 4)))
                    {
                        nota -= 2;
                    }

                    //Se tem alguma data válida
                    string dataComparar = "";
                    int cont = 0;
                    for (int i = 0; i < texto.Length; i++)
                    {
                        if (char.IsNumber(texto[i]))
                        {
                            cont++;
                        }
                        else {
                            cont = 0;
                        }

                        if (cont == 6)
                        {
                            dataComparar = texto.Substring(i - 5, 6);
                            DateTime dateTime6;
                            string formato6 = "ddMMyy";
                            if (!DateTime.TryParseExact(dataComparar, formato6, CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out dateTime6))
                            {
                                nota -= 3;
                            }
                        }
                    }

                    //Classificação
                    if (nota >= 9)
                    {
                        label17.Text = "Senha Muito Forte";
                        label17.ForeColor = Color.Green;
                        BUTvalida.Enabled = true;
                    }
                    else if (nota >= 7)
                    {
                        label17.Text = "Senha Forte";
                        label17.ForeColor = Color.Blue;
                        BUTvalida.Enabled = true;
                    }
                    else if (nota >= 5)
                    {
                        label17.Text = "Senha Razoavel";
                        label17.ForeColor = Color.OrangeRed;
                        BUTvalida.Enabled = true;
                    }
                    else if (nota >= 3)
                    {
                        label17.Text = "Senha Fraca";
                        label17.ForeColor = Color.DeepPink;
                        BUTvalida.Enabled = true;
                    }
                    else
                    {
                        label17.Text = "Senha Muito Fraca\nDigite uma mais forte";
                        label17.ForeColor = Color.Red;
                        BUTvalida.Enabled = false;
                    }

                } else { //Desabilita o botão se não tiverem sido preenchidos todos os requisitos
                    BUTvalida.Enabled = false;
                    label17.Text = " ";
                }
            }
            catch { }
        }

        private void BUTvalida_Click(object sender, EventArgs e)
        {
            usuarioSessao.AlteracaoSenha = DateTime.Now;
            TXTdata.Text = usuarioSessao.AlteracaoSenha.ToShortDateString();

            usuarioSessao.Senha = tbSenha_teste.Text;
            TXTsenha.Text = usuarioSessao.Senha;

            usuarioSessao.Status = "Normal";
            TXTstatus.Text = usuarioSessao.Status;

            BUTvalida.Enabled = false;

            MessageBox.Show("Senha trocada com sucesso\nNova Senha: " + usuarioSessao.Senha);
            atualizaUsuario();
            tbSenha_teste.Text = "";
            normal(checkBox1);
            normal(checkBox2);
            normal(checkBox3);
            normal(checkBox4);
            normal(checkBox5);
            normal(checkBox6);
            label17.Text = "";

            populaTabs();


        }
        private void populaTabs()
        {
            if (!tabCtrlLogado.Controls.Contains(tabInformacoesUsuario))
            {
                switch (usuarioSessao.Perfil)
                {
                    case 1:
                        tabCtrlTopo.Controls.Remove(tabSobre);
                        tabCtrlTopo.Controls.Remove(tabSair);

                        tabCtrlTopo.Controls.Add(tabCadas);
                        tabCtrlTopo.Controls.Add(tabBloqueados);
                        tabCtrlTopo.Controls.Add(tabEscalas);
                        tabCtrlTopo.Controls.Add(tabData);

                        tabCtrlTopo.Controls.Add(tabSobre);
                        tabCtrlTopo.Controls.Add(tabSair);

                        tabCtrlLogado.Controls.Remove(tabTrocarSenha);
                        tabCtrlLogado.Controls.Add(tabInformacoesUsuario);
                        tabCtrlLogado.Controls.Add(tabTrocarSenha);

                        break;
                    case 2:
                        tabCtrlTopo.Controls.Remove(tabSobre);
                        tabCtrlTopo.Controls.Remove(tabSair);

                        tabCtrlTopo.Controls.Add(tabMensagens);
                        tabCtrlTopo.Controls.Add(tabEscalas);
                        tabCtrlTopo.Controls.Add(tabData);

                        tabCtrlLogado.Controls.Remove(tabTrocarSenha);
                        tabCtrlLogado.Controls.Add(tabInformacoesUsuario);
                        tabCtrlLogado.Controls.Add(tabTrocarSenha);
                        break;
                    case 3:
                        tabCtrlTopo.Controls.Remove(tabSobre);
                        tabCtrlTopo.Controls.Remove(tabSair);

                        tabCtrlTopo.Controls.Add(tabMensagens);
                        tabCtrlTopo.Controls.Add(tabData);

                        tabCtrlLogado.Controls.Remove(tabTrocarSenha);
                        tabCtrlLogado.Controls.Add(tabInformacoesUsuario);
                        tabCtrlLogado.Controls.Add(tabTrocarSenha);

                        break;
                    case 4:

                        tabCtrlTopo.Controls.Remove(tabSobre);
                        tabCtrlTopo.Controls.Remove(tabSair);

                        tabCtrlTopo.Controls.Add(tabData);

                        tabCtrlLogado.Controls.Remove(tabTrocarSenha);
                        tabCtrlLogado.Controls.Add(tabInformacoesUsuario);
                        tabCtrlLogado.Controls.Add(tabTrocarSenha);
                        break;
                }
            }
        }

        private void btnLogar_Click(object sender, EventArgs e)
        {
            foreach (Usuario usuario in usuarios )
            {
                if (tbUsuario.Text == usuario.Nome && tbSenha.Text == usuario.Senha)
                {
                    usuarioSessao = usuario;
                    tabCtrlLogado.Controls.Remove(tabInformacoesUsuario);
                    if (usuarioSessao.Status == "Senha Inicial" ||  (DateTime.Now - usuarioSessao.AlteracaoSenha) >= TimeSpan.FromDays(180))
                    {
                        MessageBox.Show("Troca de senha obrigatória.");
                        tabCtrlLogado.SelectedIndex = 1;

                        tabCtrlTopo.Controls.Remove(tabSair);
                        tabCtrlTopo.Controls.Remove(tabEscalas);
                        tabCtrlTopo.Controls.Remove(tabSobre);
                        tabCtrlTopo.Controls.Remove(tabCadas);
                        tabCtrlTopo.Controls.Remove(tabEscalas);
                        tabCtrlTopo.Controls.Remove(tabData);
                        tabCtrlTopo.Controls.Remove(tabBloqueados);

                        tabCtrlLogado.Controls.Remove(tabMensagens);
                        tabCtrlLogado.Controls.Remove(tabInformacoesUsuario);

                        tabCtrlTopo.Controls.Add(tabSobre);
                        tabCtrlTopo.Controls.Add(tabSair);

                    }

                    btnLogar.Enabled = false; //Para os campos nao ficarem no fundo podendo ser focados usando o index (apertando TAB)
                    tbUsuario.Enabled = false;
                    tbSenha.Enabled = false;

                    TXTcod.Text = usuario.Id;

                    switch (usuario.Perfil)
                    {
                        case 1: TXTperfil.Text = "Administrador"; break;
                        case 2: TXTperfil.Text = "Gerente"; break;
                        case 3: TXTperfil.Text = "Operador"; break;
                        case 4: TXTperfil.Text = "Estagiário"; break;
                        default: break;
                    }

                    TXTnome.Text = usuario.Nome;
                    TXTrg.Text = usuario.Rg;
                    TXTdata.Text = usuario.AlteracaoSenha.ToLongDateString();

                    
                    
                    TXTsenha.Text = usuario.Senha;

                    //tabLogado.TabPages.Add("Tab customizada do usuario");
                    tabCtrlLogado.Visible = true;

                    populaTabs();

                    break;
                }
                else if(usuario.Equals(usuarios.Last()))
                {
                    MessageBox.Show("Login incorreto.");
                    break;
                }
            }
        }

        private void atualizaUsuario()
        {
            foreach(Usuario usuario in usuarios.ToList())
            {
                if(usuarioSessao.Id == usuario.Id)
                {
                    usuario.Id = usuarioSessao.Id;
                    usuario.Nome = usuarioSessao.Nome;
                    usuario.Perfil = usuarioSessao.Perfil;
                    usuario.Rg = usuarioSessao.Rg;
                    usuario.Senha = usuarioSessao.Senha;
                    usuario.Status = usuarioSessao.Status;
                    usuario.AlteracaoSenha = usuarioSessao.AlteracaoSenha;
                    break;
                }
            }
        }

        private void tbTempK_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                double temp1, temp2;
                temp1 = double.Parse(tbTempK.Text.ToString());
                temp1 = temp1 - 273.15;

                temp2 = double.Parse(tbTempK.Text.ToString());
                temp2 = (temp2 * 1.8) - 459.67;

                tbTempC.Text = temp1.ToString();
                tbTempF.Text = temp2.ToString();

                tbTempF.BackColor = Color.SkyBlue;
                tbTempC.BackColor = Color.SkyBlue;
                tbTempK.BackColor = Color.AliceBlue;

                int porcentagem;
                porcentagem = Convert.ToInt32((temp2 + 459.67) / (1459.67 / 100));
                if (porcentagem > 100)
                {
                    pbF.Value = 100;
                }
                else
                {
                    pbF.Value = porcentagem;
                }

                porcentagem = Convert.ToInt32(double.Parse(tbTempK.Text.ToString()) / 10);
                if (porcentagem > 100)
                {
                    pbK.Value = 100;
                }
                else
                {
                    pbK.Value = porcentagem;
                }

                porcentagem = Convert.ToInt32((temp1 + 273.15) / (1273.15 / 100));
                if (porcentagem > 100)
                {
                    pbC.Value = 100;
                }
                else
                {
                    pbC.Value = porcentagem;
                }

            }
            catch (Exception)
            {
                tbTempC.Text = "";
                tbTempF.Text = "";

                tbTempF.BackColor = Color.SkyBlue;
                tbTempC.BackColor = Color.SkyBlue;
                tbTempK.BackColor = Color.Orange;
            }

            if (tbTempK.Text.ToString() == "")
            {
                tbTempF.BackColor = Color.White;
                tbTempC.BackColor = Color.White;
                tbTempK.BackColor = Color.White;
                pbF.Value = 0;
                pbK.Value = 0;
                pbC.Value = 0;
            }
        }

        private void BTNCalcular_Click(object sender, EventArgs e)
        {
            DateTime dataSis;
            int ano, mes, dia, horas;

            try
            {
                DateTime dataNasc = Convert.ToDateTime(MTXTnasc.Text);

                dataSis = System.DateTime.Today;

                //ANO
                if (dataNasc.Month > dataSis.Month)
                {
                    ano = (dataSis.Year - dataNasc.Year) - 1;
                    LBLAnoId.Text = Convert.ToString("Ano: " + ano);
                }
                else
                {
                    if ((dataNasc.Month == dataSis.Month) && (dataNasc.Day > dataSis.Day))
                    {
                        ano = (dataSis.Year - dataNasc.Year) - 1;
                        LBLAnoId.Text = Convert.ToString("Ano: " + ano);
                    }
                    else
                    {
                        ano = dataSis.Year - dataNasc.Year;
                        LBLAnoId.Text = Convert.ToString("Ano: " + ano);
                    }

                }

                //MÊS
                mes = ano * 12;
                LBLMesId.Text = Convert.ToString("Mês: " + mes);

                //DIA
                int mes30, mes31, mesd;

                mesd = mes / 2;
                mes30 = 30 * mesd;
                mes31 = 31 * mesd;
                dia = mes30 + mes31;
                LBLDiasId.Text = Convert.ToString("Dias: " + dia);

                //HORAS
                horas = dia * 24;
                LBLHorasId.Text = Convert.ToString("Horas: " + horas);
            }

            catch {
                MessageBox.Show("Entrada inválida, por favor ultilizar o formado: DD/MM/AAAA");
                MTXTnasc.Clear();
             }
            
         }

        private void BTNCalcular1_Click(object sender, EventArgs e)
        {
            int ano, mes, semana, dias, horas;

            try{
                DateTime dataInic = Convert.ToDateTime(MTXTInicial.Text);
                DateTime dataFinal = Convert.ToDateTime(MTXTFinal.Text);

                //ANO
                if (dataInic.Month > dataFinal.Month){
                    ano = (dataFinal.Year - dataInic.Year) - 1;
                    LBLAnoDif.Text = Convert.ToString("Ano: " + ano);
                }
                else{
                    if ((dataInic.Month == dataFinal.Month) && (dataInic.Day > dataFinal.Day)){
                        ano = (dataFinal.Year - dataInic.Year) - 1;
                        LBLAnoDif.Text = Convert.ToString("Ano: " + ano);
                    }
                    else{
                        ano = dataFinal.Year - dataInic.Year;
                        LBLAnoDif.Text = Convert.ToString("Ano: " + ano);
                    }
                }

                //MES
                if (dataInic.Day > dataFinal.Day) {
                    if (dataInic.Month > dataFinal.Month) {
                        mes = (dataInic.Month - dataFinal.Month) - 1;
                        LBLMesDif.Text = Convert.ToString("Mês: " + mes);
                    }
                    else {
                        mes = (dataFinal.Month - dataInic.Month) - 1;
                        LBLMesDif.Text = Convert.ToString("Mês: " + mes);
                    }
                }
                else {
                    if (dataInic.Month > dataFinal.Month){
                        mes = dataInic.Month - dataFinal.Month;
                        LBLMesDif.Text = Convert.ToString("Mês: " + mes);
                    }
                    else{
                        mes = dataFinal.Month - dataInic.Month;
                        LBLMesDif.Text = Convert.ToString("Mês: " + mes);
                    }
                }

                //DIAS
                if (dataInic.Day > dataFinal.Day){
                    dias = dataInic.Day - dataFinal.Day ;
                    LBLdiasDif.Text = Convert.ToString("Dia: " + dias);
                }
                else{
                    dias = dataFinal.Day - dataInic.Day;
                    LBLdiasDif.Text = Convert.ToString("Dia: " + dias);
                }


                //SEMANA
                if (dias >= 7){
                    semana = dias / 7;
                    LBLSemanaDif.Text = Convert.ToString("Semana: " + semana);
                }
                else{
                    semana = 0;
                    LBLSemanaDif.Text = Convert.ToString("Semana: " + semana);
                }

                //HORAS

                horas = dias * 24;
                LBLHorasDif.Text = Convert.ToString("Horas: " + horas);
            }
            catch {
                MessageBox.Show("Entrada inválida, por favor ultilizar o formado: DD/MM/AAAA");
                MTXTInicial.Clear();
                MTXTFinal.Clear();
            }
        }

        private void BTNCalcAcres_Click(object sender, EventArgs e)
        {

            if (TXTMesAcres.Text == "")
                TXTMesAcres.Text = "0";

            if (TXTSemanaAcres.Text == "")
                TXTSemanaAcres.Text = "0";

            if (TXTDiasAcres.Text == "")
                TXTDiasAcres.Text = "0";

            try
            {
                DateTime dataInic = Convert.ToDateTime(MTXTDataAcres.Text);

                LBLDataFinalAcres.Text = dataInic.AddDays(Convert.ToInt32(TXTDiasAcres.Text))
                                        .AddMonths(Convert.ToInt32(TXTMesAcres.Text))
                                        .AddDays(7* Convert.ToInt32(TXTSemanaAcres.Text))
                                        .ToShortDateString();
            }

            catch{

            }
        }

        private void btnSubtrair_Click(object sender, EventArgs e)
        {
            if (TXTMesAcres.Text == "")
                TXTMesAcres.Text = "0";

            if (TXTSemanaAcres.Text == "")
                TXTSemanaAcres.Text = "0";

            if (TXTDiasAcres.Text == "")
                TXTDiasAcres.Text = "0";

            try
            {
                DateTime dataInic = Convert.ToDateTime(MTXTDataAcres.Text);




                LBLDataFinalAcres.Text = dataInic.AddDays(-1*Convert.ToInt32(TXTDiasAcres.Text))
                                        .AddMonths(-1*Convert.ToInt32(TXTMesAcres.Text))
                                        .AddDays(-7 * Convert.ToInt32(TXTSemanaAcres.Text))
                                        .ToShortDateString();
            }
            catch
            {

            }


        }

        private void btnCalcInfo_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime dataInfo = Convert.ToDateTime(MTXTdataInf.Text);
                var culture = new System.Globalization.CultureInfo("pt-BR");
                LBLDiaSemana.Text = "Dia da semana: " + culture.DateTimeFormat.GetDayName(dataInfo.DayOfWeek).ToString();
                LBLNomeMes.Text = "Mês: " + culture.DateTimeFormat.GetMonthName(dataInfo.Month).ToString();
                LBLDiaJuliano.Text = "Dia Juliano: " + dataInfo.DayOfYear.ToString();
                if (DateTime.IsLeapYear(dataInfo.Year))
                    LBLBissexto.Text = "Bissexto: SIM";
                else
                    LBLBissexto.Text = "Bissexto: NÃO";
            }
            catch
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            LBResCal.Items.Clear();

            DateTime data = Convert.ToDateTime(CBDatas.SelectedItem.ToString());
            HijriCalendar islamico = new HijriCalendar();
            HebrewCalendar judaico = new HebrewCalendar();
            ChineseLunisolarCalendar chines = new ChineseLunisolarCalendar();

            LBResCal.Items.Add("Islamico: " + 
                                islamico.GetDayOfMonth(data).ToString() + 
                                "/" + islamico.GetMonth(data).ToString() + "/" + 
                                islamico.GetYear(data).ToString());

            LBResCal.Items.Add("Judaico: " + 
                                judaico.GetDayOfMonth(data).ToString() + 
                                "/" + judaico.GetMonth(data).ToString() + "/" + 
                                judaico.GetYear(data).ToString());
            string animal = "";
            switch (chines.GetTerrestrialBranch(chines.GetSexagenaryYear(data)) -1 ) {
                case  0: animal = "Rato"; break;
                case  1: animal = "Boi"; break;
                case  2: animal = "Tigre"; break;
                case  3: animal = "Coelho"; break;
                case  4: animal = "Dragão"; break;
                case  5: animal = "Cobra"; break;
                case  6: animal = "Cavalo"; break;
                case  7: animal = "Cabra"; break;
                case  8: animal = "Macaco"; break;
                case  9: animal = "Galo"; break;
                case 10: animal = "Cachorro"; break;
                case 11: animal = "Porco"; break;
            }
            LBResCal.Items.Add("Chinês: " +
                                chines.GetYear(data).ToString() +
                                " animal: " + animal);

        }

        private void btnSairSim_Click(object sender, EventArgs e)
        {
            
            FormPrincipal Form = new FormPrincipal();
            Form.Show();
            this.Hide();
        }

        private void btnSairNao_Click(object sender, EventArgs e)
        {
            tabCtrlTopo.SelectTab(tabAnt);
        }

        private void tabCtrlTopo_Deselected(object sender, TabControlEventArgs e)
        {
            tabAnt = e.TabPageIndex;
        }

        private void tbTempC_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                double temp1, temp2;

                temp1 = double.Parse(tbTempC.Text.ToString());
                temp1 = (temp1 * 1.8) + 32;

                temp2 = double.Parse(tbTempC.Text.ToString());
                temp2 = temp2 + 273.15;
                tbTempF.Text = temp1.ToString();
                tbTempK.Text = temp2.ToString();

                int porcentagem;
                porcentagem = Convert.ToInt32((temp1 + 459.67) / (1459.67 / 100));
                if (porcentagem > 100)
                {
                    pbF.Value = 100;
                }
                else
                {
                    pbF.Value = porcentagem;
                }

                porcentagem = Convert.ToInt32(temp2 / 10);
                if (porcentagem > 100)
                {
                    pbK.Value = 100;
                }
                else
                {
                    pbK.Value = porcentagem;
                }

                porcentagem = Convert.ToInt32((double.Parse(tbTempC.Text.ToString()) + 273.15) / (1273.15 / 100));
                if (porcentagem > 100)
                {
                    pbC.Value = 100;
                }
                else
                {
                    pbC.Value = porcentagem;
                }

                tbTempF.BackColor = Color.SkyBlue;
                tbTempK.BackColor = Color.SkyBlue;
                tbTempC.BackColor = Color.AliceBlue;
            }
            catch (Exception)
            {
                tbTempF.Text = "";
                tbTempK.Text = "";

                tbTempF.BackColor = Color.SkyBlue;
                tbTempK.BackColor = Color.SkyBlue;
                tbTempC.BackColor = Color.Orange;
            }
            if (tbTempC.Text.ToString() == "")
            {
                tbTempF.BackColor = Color.White;
                tbTempC.BackColor = Color.White;
                tbTempK.BackColor = Color.White;
                pbF.Value = 0;
                pbK.Value = 0;
                pbC.Value = 0;
            }
        }

        private void tbTempF_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                double temp1, temp2;
                temp1 = double.Parse(tbTempF.Text.ToString());
                temp1 = (temp1 - 32) / 1.8;

                temp2 = double.Parse(tbTempF.Text.ToString());
                temp2 = (temp2 + 459.67) / 1.8;

                tbTempC.Text = temp1.ToString();
                tbTempK.Text = temp2.ToString();

                tbTempC.BackColor = Color.SkyBlue;
                tbTempK.BackColor = Color.SkyBlue;
                tbTempF.BackColor = Color.AliceBlue;

                int porcentagem;
                porcentagem = Convert.ToInt32((double.Parse(tbTempF.Text.ToString()) + 459.67) / (1459.67 / 100));
                if (porcentagem > 100)
                {
                    pbF.Value = 100;
                }
                else
                {
                    pbF.Value = porcentagem;
                }

                porcentagem = Convert.ToInt32(temp2 / 10);
                if (porcentagem > 100)
                {
                    pbK.Value = 100;
                }
                else
                {
                    pbK.Value = porcentagem;
                }

                porcentagem = Convert.ToInt32((temp1 + 273.15) / (1273.15 / 100));
                if (porcentagem > 100)
                {
                    pbC.Value = 100;
                }
                else
                {
                    pbC.Value = porcentagem;
                }

            }
            catch (Exception)
            {
                tbTempK.Text = "";
                tbTempC.Text = "";

                tbTempK.BackColor = Color.SkyBlue;
                tbTempC.BackColor = Color.SkyBlue;
                tbTempF.BackColor = Color.Orange;
            }
            if (tbTempF.Text.ToString() == "")
            {
                tbTempF.BackColor = Color.White;
                tbTempC.BackColor = Color.White;
                tbTempK.BackColor = Color.White;
                pbF.Value = 0;
                pbK.Value = 0;
                pbC.Value = 0;
            }
        }

        private void BTNalterarRG_Click(object sender, EventArgs e)
        {
            usuarioSessao.Rg = TXTrg.Text;
            usuarioSessao.Nome = TXTnome.Text;
            MessageBox.Show("Campos alterados com sucesso!");
            BTNalterarRG.Visible = false;
            button1.Enabled = true;
            TXTrg.Enabled = false;
            TXTnome.Enabled = false;
            atualizaUsuario();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BTNalterarRG.Visible = true;
            button1.Enabled = false;
            TXTrg.Enabled = true;
            TXTnome.Enabled = true;
        }
        
        private static string senhaAleatoria()
        {
            string caracteresPermitidos = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            char[] caracteres = new char[7];
            Random rd = new Random();

            for (int i = 0; i < 7; i++)
            {
                caracteres[i] = caracteresPermitidos[rd.Next(0, caracteresPermitidos.Length)];
            }

            return new string(caracteres);
        }

        private int Sequencia(string texto)
        {
            int sequencia = 0;
            for (int i = 0; i < texto.Length; i++)// percorre tudo
            {
                if (sequencia < 3)
                {
                    if (i != texto.Length - 1 && char.IsNumber(texto[i])) // se o primeiro for num
                    {
                        if (char.IsNumber(texto[i + 1])) // se o segundo for num
                        {
                            if (texto[i + 1] - texto[i] == 1) // se o segundo for o proximo num do primeiro
                            {
                                sequencia++;
                            }
                        }
                        else
                        {
                            i++;
                            sequencia = 0;
                            continue;
                        }
                    }
                }
                else
                {
                    return 2;
                }
            }
            return 0;
        }

        private void CheckVerde(CheckBox checkb)
        {
            checkb.Checked = true;
            checkb.BackColor = Color.Green;
            checkb.ForeColor = Color.White;
        }

        private void CheckVermelho(CheckBox checkb)
        {
            checkb.Checked = false;
            checkb.BackColor = Color.Red;
            checkb.ForeColor = Color.White;
        }

        private void normal(CheckBox checkb)
        {
            checkb.Checked = false;
            checkb.BackColor = SystemColors.Control;
            checkb.ForeColor = Color.Black;
        }

        private void btNovo_Click(object sender, EventArgs e)
        {
            limpaCampos();
        }

        private void limpaCampos()
        {
            tbCodigoUsuario.Text = "";
            tbNomeUsuario.Text = "";
            tbRGUsuario.Text = "";
            tbSenhaUsuario.Text = "";
            cbPerfilUsuario.SelectedIndex = -1;
            cbStatusUsuario.SelectedIndex = -1;
            cbStatusUsuario.Enabled = false;
            tbSenhaUsuario.Enabled = false;
        }

        private void btSalvarUsuario_Click(object sender, EventArgs e)
        {
            foreach(Usuario usuario in usuarios.ToList()){
                if (usuario.Id == tbCodigoUsuario.Text)
                {
                    usuario.Nome = tbNomeUsuario.Text;
                    usuario.Perfil = cbPerfilUsuario.SelectedIndex +1;
                    usuario.Rg = tbRGUsuario.Text;
                    usuario.Senha = tbSenhaUsuario.Text;
                    usuario.Status = cbStatusUsuario.Text;
                    usuario.AlteracaoSenha = DateTime.Now;
                    break;
                }else if (usuario.Equals(usuarios.Last()))
                {
                    Usuario novoUsuario = new Usuario(tbCodigoUsuario.Text, tbNomeUsuario.Text, senhaAleatoria(), cbPerfilUsuario.SelectedIndex + 1, DateTime.Now, tbRGUsuario.Text, "Senha Inicial");
                    usuarios.Add(novoUsuario);
                }
            }

            dgvUsuarios.Refresh();
            limpaCampos();
            populaBloqueados();
        }

        private void btEditar_Click(object sender, EventArgs e)
        {
            var usuarioSelecionado = dgvUsuarios.SelectedRows[0];
            
            
            tbCodigoUsuario.Text = usuarioSelecionado.Cells[0].Value.ToString();
            tbNomeUsuario.Text = usuarioSelecionado.Cells[1].Value.ToString();
            tbSenhaUsuario.Enabled = true;
            tbSenhaUsuario.Text = usuarioSelecionado.Cells[2].Value.ToString();
            cbPerfilUsuario.SelectedIndex = int.Parse(usuarioSelecionado.Cells[3].Value.ToString()) - 1;
            tbDataSenhaUsuario.Text = usuarioSelecionado.Cells[4].Value.ToString();
            tbRGUsuario.Text = usuarioSelecionado.Cells[5].Value.ToString();
            cbStatusUsuario.Enabled = true;
            if (usuarioSelecionado.Cells[6].Value.ToString().Equals("Senha Inicial"))
            {
                cbStatusUsuario.SelectedIndex = 2;
            }
            else
            {
                cbStatusUsuario.Text = usuarioSelecionado.Cells[6].Value.ToString();
            }
        }

        private void btResetarSenha_Click(object sender, EventArgs e)
        {
            var usuarioSelecionado = dgvUsuarios.SelectedRows[0];

            foreach(Usuario usuario in usuarios.ToList())
            {
                if(usuario.Id == usuarioSelecionado.Cells[0].Value.ToString())
                {
                    usuario.Senha = senhaAleatoria();
                    usuario.AlteracaoSenha = DateTime.Now;
                    usuario.Status = "Senha Inicial";
                    dgvUsuarios.Refresh();
                    break;
                }
            }
        }
        private void populaBloqueados()
        {
            this.dgvBloqueados.Rows.Clear();
            foreach (Usuario usuario in usuarios.ToList())
            {
                if (usuario.Status == "Bloqueado")
                    dgvBloqueados.Rows.Add(usuario.Id, usuario.Nome, usuario.Senha, usuario.Perfil, usuario.AlteracaoSenha, usuario.Rg, usuario.Status);
            }
        }

        private void btRemover_Click(object sender, EventArgs e)
        {
            var usuarioSelecionado = dgvUsuarios.SelectedRows[0];

            foreach (Usuario usuario in usuarios.ToList())
            {
                if (usuario.Id == usuarioSelecionado.Cells[0].Value.ToString())
                {
                    usuarios.Remove(usuario);
                    dgvUsuarios.Refresh();
                    populaBloqueados();
                    break;
                }

            }
        }

        private void FormPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void tbTextoCrip_TextChanged(object sender, EventArgs e)
        {

            if (System.Text.RegularExpressions.Regex.IsMatch(tbTextoCrip.Text, "[^A-Z^a-z^0-9^ ]"))
            {
                MessageBox.Show("O texto pode conter apenas letras, algarismos numéricos e espaços em branco.");

                foreach (Match m in Regex.Matches(tbTextoCrip.Text, "[^A-Z^a-z^0-9^ ]"))
                    tbTextoCrip.Text = tbTextoCrip.Text.Remove(m.Index,1);

                tbTextoCrip.SelectionStart = tbTextoCrip.TextLength;
            }
            tbCriptografado.Text = criptografar();
            tbDescriptografado.Text = descriptografar();
        }

        private String criptografar()
        {
            textoCriptografado = "";
            int i = 0;
            foreach (char caractere in tbTextoCrip.Text.ToCharArray())
            {
                int j = 0;
                foreach(char charcifra in cifra)
                {
                    if (caractere.ToString().ToLower() == charcifra.ToString())
                    {
                        int numcifra = j + Convert.ToInt32(upDownChave.Value);
                        if(numcifra > 36)
                        {
                            numcifra -= 37;
                        }
                        switch (cifra[numcifra])
                        {
                            case 'a':
                                textoCriptografado = textoCriptografado.Insert(textoCriptografado.Length, "*");
                                break;
                            case 'e':
                                textoCriptografado = textoCriptografado.Insert(textoCriptografado.Length, "#");
                                break;
                            case 'i':
                                textoCriptografado = textoCriptografado.Insert(textoCriptografado.Length, "+");
                                break;
                            case 'o':
                                textoCriptografado = textoCriptografado.Insert(textoCriptografado.Length, "-");
                                break;
                            case 'u':
                                textoCriptografado = textoCriptografado.Insert(textoCriptografado.Length, "$");
                                break;
                            default:
                                textoCriptografado = textoCriptografado.Insert(textoCriptografado.Length, cifra[numcifra].ToString());
                                break;
                        }
                        break;
                    }
                    j++;
                }
                i++;
            }
            return textoCriptografado;
        }

        private String substituirCaracteresEspeciais(String letra)
        {
            int j = 0;
            foreach (char charcifra in cifra)
            {
                if (letra == charcifra.ToString())
                {
                    int numcifra = j - Convert.ToInt32(upDownChave.Value);
                    if (numcifra < 0)
                    {
                        numcifra = numcifra + 37;
                    }
                    return textoCriptografado.Insert(textoCriptografado.Length, cifra[numcifra].ToString());
                }
                j++;
            }
            return "";
        }

        private String descriptografar()
        {
            textoCriptografado = "";
            int i = 0;
            foreach (char caractere in tbCriptografado.Text.ToCharArray())
            {
                int j;
                switch (caractere)
                {
                    case '*':
                        textoCriptografado = substituirCaracteresEspeciais("a");
                    break;
                    case '#':
                        textoCriptografado = substituirCaracteresEspeciais("e");
                    break;
                    case '+':
                        textoCriptografado = substituirCaracteresEspeciais("i");
                    break;
                    case '-':
                        textoCriptografado = substituirCaracteresEspeciais("o");
                    break;
                    case '$':
                        textoCriptografado = substituirCaracteresEspeciais("u");
                    break;
                    default:
                        textoCriptografado = substituirCaracteresEspeciais(caractere.ToString());
                     break;
                }
                i++;
            }
            return textoCriptografado;
        }

        private void upDownChave_ValueChanged(object sender, EventArgs e)
        {
            tbCriptografado.Text = criptografar();
            tbDescriptografado.Text = descriptografar();
        }
    }
}
