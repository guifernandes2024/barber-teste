using System.ComponentModel.DataAnnotations;

namespace BarberShopApp.Core.Models
{
    public class ConfiguracaoBarbearia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da barbearia é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string NomeBarbearia { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string Telefone { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "O site deve ter no máximo 200 caracteres")]
        public string? Site { get; set; }

        [StringLength(200, ErrorMessage = "O Instagram deve ter no máximo 200 caracteres")]
        public string? Instagram { get; set; }

        [StringLength(200, ErrorMessage = "O Facebook deve ter no máximo 200 caracteres")]
        public string? Facebook { get; set; }

        [StringLength(200, ErrorMessage = "O WhatsApp deve ter no máximo 200 caracteres")]
        public string? WhatsApp { get; set; }

        // Horários de funcionamento
        [Required(ErrorMessage = "O horário de abertura é obrigatório")]
        public TimeSpan HorarioAbertura { get; set; } = new TimeSpan(8, 0, 0);

        [Required(ErrorMessage = "O horário de fechamento é obrigatório")]
        public TimeSpan HorarioFechamento { get; set; } = new TimeSpan(18, 0, 0);

        // Dias de funcionamento
        public bool SegundaAberta { get; set; } = true;
        public bool TercaAberta { get; set; } = true;
        public bool QuartaAberta { get; set; } = true;
        public bool QuintaAberta { get; set; } = true;
        public bool SextaAberta { get; set; } = true;
        public bool SabadoAberta { get; set; } = true;
        public bool DomingoAberta { get; set; } = false;

        // Horários específicos por dia (opcional)
        public TimeSpan? SegundaAbertura { get; set; }
        public TimeSpan? SegundaFechamento { get; set; }
        public TimeSpan? TercaAbertura { get; set; }
        public TimeSpan? TercaFechamento { get; set; }
        public TimeSpan? QuartaAbertura { get; set; }
        public TimeSpan? QuartaFechamento { get; set; }
        public TimeSpan? QuintaAbertura { get; set; }
        public TimeSpan? QuintaFechamento { get; set; }
        public TimeSpan? SextaAbertura { get; set; }
        public TimeSpan? SextaFechamento { get; set; }
        public TimeSpan? SabadoAbertura { get; set; }
        public TimeSpan? SabadoFechamento { get; set; }
        public TimeSpan? DomingoAbertura { get; set; }
        public TimeSpan? DomingoFechamento { get; set; }

        // Configurações de agendamento
        [Range(15, 120, ErrorMessage = "O intervalo deve ser entre 15 e 120 minutos")]
        public int IntervaloAgendamentoMinutos { get; set; } = 30;

        [Range(1, 30, ErrorMessage = "O prazo mínimo deve ser entre 1 e 30 dias")]
        public int PrazoMinimoAgendamentoDias { get; set; } = 1;

        [Range(1, 90, ErrorMessage = "O prazo máximo deve ser entre 1 e 90 dias")]
        public int PrazoMaximoAgendamentoDias { get; set; } = 30;

        // Informações adicionais
        [StringLength(500, ErrorMessage = "As políticas devem ter no máximo 500 caracteres")]
        public string? PoliticasCancelamento { get; set; }

        [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
        public string? Observacoes { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime DataAtualizacao { get; set; } = DateTime.Now;

        // Métodos auxiliares
        public bool EstaAberta(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => SegundaAberta,
                DayOfWeek.Tuesday => TercaAberta,
                DayOfWeek.Wednesday => QuartaAberta,
                DayOfWeek.Thursday => QuintaAberta,
                DayOfWeek.Friday => SextaAberta,
                DayOfWeek.Saturday => SabadoAberta,
                DayOfWeek.Sunday => DomingoAberta,
                _ => false
            };
        }

        public TimeSpan ObterHorarioAbertura(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => SegundaAbertura ?? HorarioAbertura,
                DayOfWeek.Tuesday => TercaAbertura ?? HorarioAbertura,
                DayOfWeek.Wednesday => QuartaAbertura ?? HorarioAbertura,
                DayOfWeek.Thursday => QuintaAbertura ?? HorarioAbertura,
                DayOfWeek.Friday => SextaAbertura ?? HorarioAbertura,
                DayOfWeek.Saturday => SabadoAbertura ?? HorarioAbertura,
                DayOfWeek.Sunday => DomingoAbertura ?? HorarioAbertura,
                _ => HorarioAbertura
            };
        }

        public TimeSpan ObterHorarioFechamento(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Monday => SegundaFechamento ?? HorarioFechamento,
                DayOfWeek.Tuesday => TercaFechamento ?? HorarioFechamento,
                DayOfWeek.Wednesday => QuartaFechamento ?? HorarioFechamento,
                DayOfWeek.Thursday => QuintaFechamento ?? HorarioFechamento,
                DayOfWeek.Friday => SextaFechamento ?? HorarioFechamento,
                DayOfWeek.Saturday => SabadoFechamento ?? HorarioFechamento,
                DayOfWeek.Sunday => DomingoFechamento ?? HorarioFechamento,
                _ => HorarioFechamento
            };
        }

        public bool PodeAgendar(DateTime data)
        {
            var hoje = DateTime.Today;
            var prazoMinimo = hoje.AddDays(PrazoMinimoAgendamentoDias);
            var prazoMaximo = hoje.AddDays(PrazoMaximoAgendamentoDias);

            return data.Date >= prazoMinimo && 
                   data.Date <= prazoMaximo && 
                   EstaAberta(data.DayOfWeek);
        }
    }
} 