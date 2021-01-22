using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JqueryDataTable.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JqueryDataTable.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        // POST api/<controller>
        [HttpPost]
        public IActionResult Post()
        {
            var requestFormData = Request.Form;

            List<Item> listaItens = GetItens();

            var listaItensForm = ProcessarDadosForm(listaItens, requestFormData);

            dynamic response = new
            {
                Data = listaItensForm,
                Draw = requestFormData["draw"],
                RecordsFiltered = listaItens.Count(),
                RecordsTotal = listaItens.Count()
            };
            return Ok(response);
        }

        private List<Item> GetItens()
        {
            List<Item> listaItens = new List<Item>()
            {
                new Item() {  ItemId = 1000, Nome="Caneta Esferográfica", Descricao="Caneta Azul bic Pequena" },
                new Item() {  ItemId = 1000, Nome="Lapis", Descricao="Lapis Preto" },
                new Item() {  ItemId = 1000, Nome="Caderno 100", Descricao="Caderno Espiral 100" },
                new Item() {  ItemId = 1000, Nome="Caderno 200", Descricao="Caderno Espiral 200" },
                new Item() {  ItemId = 1000, Nome="Caderno 300", Descricao="Caderno Espiral 300" },
                new Item() {  ItemId = 1000, Nome="Caderno 400", Descricao="Caderno Espiral 400" },
                new Item() {  ItemId = 1000, Nome="Caneta Tinteiro", Descricao="Caneta pena" },
                new Item() {  ItemId = 1000, Nome="Caderno 500", Descricao="Caderno Espiral 500" },
                new Item() {  ItemId = 1000, Nome="Caderno 600", Descricao="Caderno Espiral 600" },
                new Item() {  ItemId = 1000, Nome="Caderno 700", Descricao="Caderno Espiral 700" },
                new Item() {  ItemId = 1000, Nome="Caneta xxxxxx", Descricao="Caneta xxxxxxxxxxx" },
            };
            return listaItens;
        }

        private List<Item> ProcessarDadosForm(List<Item> lstElements, IFormCollection requestFormData)
        {
            var skip = Convert.ToInt32(requestFormData["start"].ToString());
            var pageSize = Convert.ToInt32(requestFormData["length"].ToString());
            Microsoft.Extensions.Primitives.StringValues tempoOrder = new [] { "" };

            if(requestFormData.TryGetValue("order[0][column]",out tempoOrder))
            {
                var columnIndex = requestFormData["order[0][column]"].ToString();
                var sortDirection = requestFormData["order[0][dir]"].ToString();
                tempoOrder = new[] { "" };
                if(requestFormData.TryGetValue($"columns[{columnIndex}][data]", out tempoOrder))
                {
                    var columnName = requestFormData[$"columns[{columnIndex}][data]"].ToString();

                    if (pageSize > 0)
                    {
                        var prop = GetProperty(columnName);
                        if (sortDirection == "asc")
                        {
                            return lstElements.OrderBy(prop.GetValue).Skip(skip)
                                .Take(pageSize).ToList();
                        }
                        else
                            return lstElements.OrderByDescending(prop.GetValue)
                                .Skip(skip).Take(pageSize).ToList();
                    }
                    else
                        return lstElements;
                }
            }
            return null;
        }

        private PropertyInfo GetProperty(string name)
        {
            var properties = typeof(Models.Item).GetProperties();
            PropertyInfo prop = null;
            foreach(var item in properties)
            {
                if(item.Name.ToLower().Equals(name.ToLower()))
                {
                    prop = item;
                    break;
                }
            }
            return prop;
        }

    }
}
