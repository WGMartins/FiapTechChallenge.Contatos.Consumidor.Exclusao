using Microsoft.AspNetCore.Mvc;
using UseCase.ContatoUseCase.Adicionar;
using UseCase.Interfaces;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ContatoController : ControllerBase
    {
        private readonly IAdicionarContatoUseCase _adicionarContatoUseCase;

        public ContatoController(IAdicionarContatoUseCase adicionarContatoUseCase)
        {
            _adicionarContatoUseCase = adicionarContatoUseCase;
        }

        [HttpPost]
        public IActionResult Adicionar(AdicionarContatoDto adicionarContatoDto)
        {
            try
            {
                return Ok(_adicionarContatoUseCase.Adicionar(adicionarContatoDto));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
