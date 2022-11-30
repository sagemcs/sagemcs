$(document).ready(function () 
{

});

function TipoC() {
        
    var titulo;
    var Cambio;
    var Fechas;

    // Tipos de Cambios
    var Canad;
    var Ante;
    var Act;
    var EUR;
    var YEN;
    var GBP;

    // Fechas al Tipo de Cambios
    var FCanad;
    var FAnte;
    var FAct;
    var FEUR;
    var FYEN;
    var FGBP;

    if (typeof Cambio == 'undefined') {
        $.ajax({
            url: 'https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF60653,SF43718,SF46410,SF60632,SF46406,SF46407/datos/oportuno?token=6d3bb969429ac7d4c13ab74bdbdde844eb557d071733e3e117ba4973d30d26c5',
            jsonp: 'callback',
            dataType: 'jsonp', //Se utiliza JSONP para realizar la consulta cross-site
            success: function (response) {  //Handler de la respuesta
                var series = response.bmx.series;


                var span = document.createElement("span");
                
                //Se carga una tabla con los registros obtenidos
                for (var i in series) {
                    var serie = series[i];

                    if (serie.idSerie == "SF60653") // -- Anterior
                    {
                        Ante = serie.datos[0].dato;
                        FAnte = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF43718") // -- Actual
                    {
                        Act = serie.datos[0].dato;
                        FAct = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF46410") // -- EURO
                    {
                        EUR = serie.datos[0].dato;
                        FEUR = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF60632") // --Canadiese
                    {
                        Canad = serie.datos[0].dato;
                        FCanad = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF46406") // --YEN
                    {
                        YEN = serie.datos[0].dato;
                        FYEN = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF46407") // --GBP
                    {
                        GBP = serie.datos[0].dato;
                        FGBP = serie.datos[0].fecha;
                    }
                } // Cierra For

                // construye cadena de resultados
                span.innerHTML = 'Tipo de Cambio Dolar (Anterior):  $' + Act + '<br />Evaluado a la fecha: ' + FAct + '<br /><br />' +
                'Tipo de Cambio Dolar:  $' + Ante + '<br />Evaluado a la fecha: ' + FAnte + '<br /><br />' +
                'Tipo de Cambio Euro:  $' + EUR + '<br />Evaluado a la fecha: ' + FEUR + '<br /><br />' +
                'Tipo de Cambio Dolar Canadiense:  $' + Canad + '<br />Evaluado a la fecha: ' + FCanad + '<br /><br />' +
                'Tipo de Cambio YEN:  $' + YEN + '<br />Evaluado a la fecha: ' + FYEN + '<br /><br />' +
                'Tipo de Cambio GBP:  $' + GBP + '<br />Evaluado a la fecha: ' + FGBP + '<br /><br />';

                //Envia alerta
                swal({
                    title: "Tasas de Cambio",
                    type: 'info',
                    content: span,
                    confirmButtonText: "Entendido",
                });
                //swal(titulo, mensaje, "success");

            }
        });         
        }
            
    return true;
}


function TipoCXML() {

    var titulo;
    var Cambio;
    var Fechas;

    // Tipos de Cambios
    var Canad;
    var Ante;
    var Act;
    var EUR;
    var YEN;
    var GBP;

    // Fechas al Tipo de Cambios
    var FCanad;
    var FAnte;
    var FAct;
    var FEUR;
    var FYEN;
    var FGBP;

    if (typeof Cambio == 'undefined') {
        $.ajax({
            url: 'https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF60653,SF43718,SF46410,SF60632,SF46406,SF46407/datos/oportuno?token=6d3bb969429ac7d4c13ab74bdbdde844eb557d071733e3e117ba4973d30d26c5',
            jsonp: 'callback',
            dataType: 'jsonp', //Se utiliza JSONP para realizar la consulta cross-site
            success: function (response) {  //Handler de la respuesta
                var series = response.bmx.series;


                var span = document.createElement("span");

                //Se carga una tabla con los registros obtenidos
                for (var i in series) {
                    var serie = series[i];

                    if (serie.idSerie == "SF60653") // -- Anterior
                    {
                        Ante = serie.datos[0].dato;
                        FAnte = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF43718") // -- Actual
                    {
                        Act = serie.datos[0].dato;
                        FAct = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF46410") // -- EURO
                    {
                        EUR = serie.datos[0].dato;
                        FEUR = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF60632") // --Canadiese
                    {
                        Canad = serie.datos[0].dato;
                        FCanad = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF46406") // --YEN
                    {
                        YEN = serie.datos[0].dato;
                        FYEN = serie.datos[0].fecha;
                    }

                    if (serie.idSerie == "SF46407") // --GBP
                    {
                        GBP = serie.datos[0].dato;
                        FGBP = serie.datos[0].fecha;
                    }
                } // Cierra For

                // construye cadena de resultados
                span.innerHTML = 'Tipo de Cambio Dolar (Anterior):  $' + Ante + '<br />Evaluado a la fecha: ' + FAnte + '<br /><br />' +
                'Tipo de Cambio Dolar:  $' + Act + '<br />Evaluado a la fecha: ' + FAct + '<br /><br />' +
                'Tipo de Cambio Euro:  $' + EUR + '<br />Evaluado a la fecha: ' + FEUR + '<br /><br />' +
                'Tipo de Cambio Dolar Canadiense:  $' + Canad + '<br />Evaluado a la fecha: ' + FCanad + '<br /><br />' +
                'Tipo de Cambio YEN:  $' + YEN + '<br />Evaluado a la fecha: ' + FYEN + '<br /><br />' +
                'Tipo de Cambio GBP:  $' + GBP + '<br />Evaluado a la fecha: ' + FGBP + '<br /><br />';

                //Envia alerta
                swal({
                    title: "Tasas de Cambio",
                    type: 'info',
                    content: span,
                    confirmButtonText: "Entendido",
                });
                //swal(titulo, mensaje, "success");

            }
        });
    }

    return true;
}

function getDataWebService() {
    // Variables - Locales
    var oDataWebService = null;
    var oResponseXML = null;
    var oResponseJSON = null;
    var oUrlService = null;
    var oRequestParams = null;
    var oHeaders = null;
    var oTiposCambioJSON = null;
    var oXMLWebServiceBaxico = null;
    var oFileId = null;
    var oFileIdDelete = null;
    var oFileXMLBackup = null;
    var oFileCSVDelete = null;
    var oProveedorServicio = null;
    var oTypeCoinConvertParams = null;
    var isContent = false;

    // Consumimos - WebService
    try {
        // Vaciamos - Parametros
        //oFileXMLBackup = nlapiGetContext().getSetting('SCRIPT', 'custscript_l1bre_param_field_bakxmlfile');
        //oFileCSVDelete = nlapiGetContext().getSetting('SCRIPT', 'custscript_l1bre_param_field_delcsvfile');
        //oProveedorServicio = nlapiGetContext().getSetting('SCRIPT', 'custscript_proveedor_de_servicio');
        //oTypeCoinConvertParams = nlapiGetContext().getSetting('SCRIPT', 'custscript_l1bre_param_field_typescurren');

        oProveedorServicio = 2;
        oFileXMLBackup = 'T';
        oFileCSVDelete = 'T';

        // Evaluamos - Proveedor del Servicio
        // Banxico
        if (oProveedorServicio == 2 || oProveedorServicio == 1) {
            // Obtenemos - Parametros de Conexion
            oUrlService = __getURLService(2);
            oRequestParams = __getRequestParams(2);
            oHeaders = __getHeaders(2);

            // Obtenemos - Datos - WebService
            oDataWebService = nlapiRequestURL(oUrlService, oRequestParams, oHeaders);

            // Vaciamos - Respuesta - WebService
            oResponseXML = nlapiStringToXML(oDataWebService.getBody());
            // Vaciamos - XML - Respuesta - WebService - Baxico
            oXMLWebServiceBaxico = __getObtResultXML(oResponseXML);



            // Obtenemos - Tipos de Cambio - WebService

            //--oTiposCambioJSON = __getObtTiposCambio(oXMLWebServiceBaxico, 2);


            // Creamos - Archivo CSV - Tipos de Cambio

            //--oFileId = __setCreateFileImportCSV(oTiposCambioJSON, 2);


            // Actualizamos - Tipos de Cambio - Currency Exchange Rates
            
            //--__setCreateUpdateCurExchangeRates(oFileId);

            // Evauamos - Creación de Archivo - XML - Banxico
            if (oFileXMLBackup == 'T') {
                // Creamos - Archivo de Respaldo - XML - Banxico
                __setCreateBackupFileXML(oXMLWebServiceBaxico);
            }

            // Evaluamos - Eliminación de Archivo - CSV Importado
            if (oFileCSVDelete != 'T') {
                // Eliminamos - Archivo de Carga
                oFileIdDelete = nlapiDeleteFile(oFileId);
            }
        }

        // Banco de Canada
        if (oProveedorServicio == 3 || oProveedorServicio == 1) {
            // Obtenemos - Parametros de Conexion
            oUrlService = __getURLService(3);
            oRequestParams = __getRequestParams(3);
            oHeaders = __getHeaders(3);

            // Armamos - URL - Servicio
            oUrlService = oUrlService + oRequestParams;
            // Obtenemos - Datos - WebService
            oDataWebService = nlapiRequestURL(oUrlService, null, oHeaders);
            // Vaciamos - Respuesta - Web Service
            oResponseJSON = JSON.parse(oDataWebService.getBody().replace(/\n/g, ""));
            // Evaluamos - Contenido - Servicio
            if (oResponseJSON.observations.length > 0) {
                // Vaciamos - Respuesta - WebService - Banco de Canada
                oTiposCambioJSON = oResponseJSON;
                // Cambiamos - Valor
                isContent = true;
            }

            // Evaluamos - Existe - Contenido - Servicio
            if (isContent) {
                // Obtenemos - Tipos de Cambio - WebService
                oTiposCambioJSON = __getObtTiposCambio(oXMLWebServiceBaxico, 3, oTiposCambioJSON);
                // Creamos - Archivo CSV - Tipos de Cambio
                oFileId = __setCreateFileImportCSV(oTiposCambioJSON, 3);
                // Actualizamos - Tipos de Cambio - Currency Exchange Rates
                __setCreateUpdateCurExchangeRates(oFileId);

                // Evauamos - Creación de Archivo - XML - Banxico
                if (oFileXMLBackup == 'T') {
                    // Creamos - Archivo de Respaldo - XML - Banxico
                    __setCreateBackupFileJSON(JSON.stringify(oTiposCambioJSON));
                }

                // Evaluamos - Eliminación de Archivo - CSV Importado
                if (oFileCSVDelete != 'T') {
                    // Eliminamos - Archivo de Carga
                    oFileIdDelete = nlapiDeleteFile(oFileId);
                }
            }
        }

        // Mensaje - Consola
        nlapiLogExecution('DEBUG', 'Success', 'El Schedule fue ejecutado con exito!');
    } catch (e) {
        // Mensaje - Consola
        nlapiLogExecution('DEBUG', 'Error: ', e.message);
    }
}

function __getURLService(oProveedorServicio) {
    // Variables - Locales
    var oUrlService = null;
    var oMessageException = null;

    // Definimos - Service
    try {
        // Evaluamos - Proveedor del Servicio
        // Banxico
        if (oProveedorServicio == 2) {
            // Vaciamos - URL - Service
            //oUrlService = "http://www.banxico.org.mx/DgieWSWeb/DgieWS";
            //oUrlService = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF60653,SF43718,SF46410,SF60632,SF46406,SF46407";
            oUrlService = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/SF60653";
        }

        // Banco de Canada
        if (oProveedorServicio == 3) {
            // Vaciamos - URL - Service
            oUrlService = "https://www.bankofcanada.ca/valet/observations/";
        }

        // Retornamos - Valor
        return oUrlService;
    } catch (e) {
        // Almacenamos - Mensaje - Excepción
        oMessageException = '[Función: __getURLService()] Ocurrio un error al momento de obtener la url del servicio: ' + e.message;
        // Lanzamos - Excepción
        throw oMessageException;
    }
}

function __getRequestParams(oProveedorServicio) {
    // Variables - Locales
    var oParamWebService = "";
    var oMessageException = null;
    var oDate = null;
    var oDia = null;
    var oMes = null;
    var oAnio = null;
    var oFechaActual = null;


    // Definimos - Parametros
    try {
        // Evaluamos - Proveedor del Servicio
        // Banxico
        if (oProveedorServicio == 2) {
            // Vaciamos - Parametros - WebService
            oParamWebService = '<soapenv:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:ws="http://ws.dgie.banxico.org.mx">' +
                               '  <soapenv:Header/>' +
                               '  <soapenv:Body>' +
                               '    <ws:tiposDeCambioBanxico soapenv:encodingStyle="http://schemas.xmlsoap.org/soap/encoding/"/>' +
                               '  </soapenv:Body>' +
                               '</soapenv:Envelope>';
        }

        // Banco de Canada
        if (oProveedorServicio == 3) {
            // Instanciamos - Objetos
            oDate = nlapiStringToDate(nlapiDateToString(new Date(), 'datetime'));

            // Separamos - Datos - Fecha
            oDia = (parseInt(oDate.getDate()) < 10 ? "0" + (oDate.getDate() - 1) : (parseInt(oDate.getDate()) - 1));
            oMes = (parseInt(oDate.getMonth() + 1) < 10 ? "0" + parseInt(oDate.getMonth() + 1) : parseInt(oDate.getMonth() + 1));
            oAnio = parseInt(oDate.getFullYear());

            // Vaciamos - Fecha de Busqueda
            oFechaActual = oAnio + "-" + oMes + "-" + oDia;

            // Vaciamos - Parametros - Servicio
            oParamWebService = oParamWebService + "FXCADUSD,FXUSDCAD" + "/json?start_date=" + oFechaActual + "&end_date=" + oFechaActual;
        }


        // Retornamos - Valor
        return oParamWebService;
    } catch (e) {
        // Almacenamos - Mensaje - Excepción
        oMessageException = '[Función: __getRequestParams()] Ocurrio un error al momento de obtener los parametros: ' + e.message;
        // Lanzamos - Excepción
        throw oMessageException;
    }
}

function __getHeaders(oProveedorServicio) {
    // Variables - Locales
    var oHeaders = null;
    var oCredentials = null;
    var oMessageException = null;

    // Definimos - Header
    try {
        // Instanciamos - Objetos
        oHeaders = new Array();


        // Evaluamos - Proveedor del Servicio
        // Banxico
        if (oProveedorServicio == 2) {
            // Seteamos - Valores - Header
            oHeaders['SOAPAction'] = 'tiposDeCambioBanxico';
            oHeaders['User-Agent-x'] = 'SuiteScript-Call';



        }

        // Banco de Canada
        if (oProveedorServicio == 3) {
            // Seteamos - Valores - Header
            oHeaders['Content-Type'] = "application/json";
        }

        // Retornamos - Valor
        return oHeaders;
    } catch (e) {
        // Almacenamos - Mensaje - Excepción
        oMessageException = '[Función: __getHeaders()] Ocurrio un error al momento de obtener el header: ' + e.message;
        // Lanzamos - Excepción
        throw oMessageException;
    }
}

function __getObtResultXML(oResponseParam) {
    // Variables - Locales
    var oResponseXML = null;
    var oXmlResult = null;
    var oMessageException = null;

    // Obtenemos - Info - Nodos
    try {
        // Obtenemos - Nodos con Información
        oResponseXML = nlapiSelectNodes(oResponseParam, '/soapenv:Envelope/soapenv:Body/ns1:tiposDeCambioBanxicoResponse');
        // Vaciamos - Result - XML
        oXmlResult = nlapiSelectValue(oResponseXML[0], 'result');

        // Retornamos - Valor
        return oXmlResult;
    } catch (e) {
        // Almacenamos - Mensaje - Excepción
        oMessageException = '[Función: __getObtInfoNodes()] Ocurrio un error al momento de obtener la información del xml: ' + e.message;
        // Lanzamos - Excepcion
        throw oMessageException;
    }
}

function __getObtTiposCambio(oResponseXML, oProveedorServicio, oTiposCambioJSON) {
    // Variables - Locales
    var oStringResultXML = null;
    var oXmlDataTipoCambio = null;
    var oNumNodes = null;
    var oObjectTiposCambioArray = null;
    var oObjectTiposCambioJSON = null;
    var oObjectTipoCambio = null;
    var oXmlResult = null;
    var oFechaParam = null;
    var oMessageError = null;
    var oMessageException = null;
    var oDate = null;
    var oTimePeriodDateFormat = null;
    var oDia = null;
    var oMes = null;
    var oAnio = null;
    var oDecimalesRedondeoBase = null;
    var oDecimalesRedondeoBaseInvertido = null;
    var oReferenceTypeCoin = null;
    var oDate = null;
    var oTimePeriod = null;

    // Obtenemos - Información - Nodos
    try {
        // Obtenemos - Parametros
        oDecimalesRedondeoBase = nlapiGetContext().getSetting('SCRIPT', 'custscript_param_field_roundbase');
        oDecimalesRedondeoBaseInvertido = nlapiGetContext().getSetting('SCRIPT', 'custscript_param_field_roundbase_invert');

        // Evaluamos - Proveedor del Servicio
        // Banxico
        if (oProveedorServicio == 2) {
            // Instanciamos - Objetos
            oObjectTiposCambioArray = new Array();

            // Reemplazamos - Tags - XML - Banxico
            oXmlResult = oResponseXML.replace(new RegExp('\r?\n', 'g'), ' ').replace(new RegExp('> <', 'g'), '><').replace('<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>', '').replace(new RegExp('bm:DataSet', 'g'), 'DataSet').replace(new RegExp('bm:Series', 'g'), 'Series').replace(new RegExp('bm:Obs', 'g'), 'Obs');
            // Extraemos - Información - Tipos de Cambio
            oStringResultXML = nlapiStringToXML(oXmlResult);
            // Recorremos - Estructura XML - Tipos de Cambio
            oXmlDataTipoCambio = nlapiSelectNodes(oStringResultXML, '/nlapi:CompactData/nlapi:DataSet/nlapi:Series/nlapi:Obs');
            // Vaciamos - Numero de Nodos - Encontrados
            oNumNodes = oXmlDataTipoCambio.length;
            // Recorremos - Nodos - Tipos de Cambio
            // Ciclo FOR
            for (var i = 0; i < oNumNodes; i++) {
                // Obtenemos - Fecha - Tipo de Cambio
                oFechaParam = nlapiSelectValue(oXmlDataTipoCambio[i], '@TIME_PERIOD').replace(new RegExp('-', 'g'), '/');
                // Cambiamos - Formato de Fecha
                oFechaParam = nlapiDateToString(new Date(oFechaParam));
                // Vaciamos - Referencia - Tipo de Cambio
                oReferenceTypeCoin = nlapiSelectValue(oXmlDataTipoCambio[i], '../@IDSERIE');

                // Vaciamos - Fecha - Formato - Import CSV
                oTimePeriodDateFormat = oFechaParam;

                // Seteamos - Elemento
                oObjectTipoCambio = {
                    TIME_PERIOD: oTimePeriodDateFormat,
                    OBS_VALUE: (1 / parseFloat(nlapiSelectValue(oXmlDataTipoCambio[i], '@OBS_VALUE'))).toFixed(oDecimalesRedondeoBaseInvertido),
                    OBS_VALUE_WS: parseFloat(nlapiSelectValue(oXmlDataTipoCambio[i], '@OBS_VALUE')).toFixed(oDecimalesRedondeoBase),
                    TYPECOIN: __getTypeCoin(oReferenceTypeCoin, oProveedorServicio)
                }
                // Vaciamos - Objeto
                oObjectTiposCambioArray.push(oObjectTipoCambio);
            }

            // Convertimos - Objeto a JSON
            oObjectTiposCambioJSON = JSON.parse(JSON.stringify(oObjectTiposCambioArray));
        }


        // Banco de Canada
        if (oProveedorServicio == 3) {
            // Instanciamos - Objetos
            oObjectTiposCambioArray = new Array();
            oDate = nlapiStringToDate(nlapiDateToString(new Date(), 'datetime'));

            // Generamos - Time Period - Tipo de Cambio (Forzamos)
            oTimePeriod = parseInt(oDate.getMonth() + 1) + "/" + parseInt(oDate.getDate()) + "/" + parseInt(oDate.getFullYear());
            //oTimePeriod =  parseInt(oDate.getDate()) + "/" + parseInt(oDate.getMonth() + 1) + "/" + parseInt(oDate.getFullYear());


            // Recorremos - Nodos - Tipos de Cambio
            // Ciclo FOR
            for (var i = 0; i < oTiposCambioJSON.observations.length; i++) {
                // Seteamos - Elemento - FXUSDCAD
                oObjectTipoCambio = {
                    TIME_PERIOD: oTimePeriod,
                    OBS_VALUE: (1 / parseFloat(oTiposCambioJSON.observations[i].FXUSDCAD.v)).toFixed(oDecimalesRedondeoBaseInvertido),
                    OBS_VALUE_WS: parseFloat(oTiposCambioJSON.observations[i].FXUSDCAD.v).toFixed(oDecimalesRedondeoBaseInvertido),
                    TYPECOIN: __getTypeCoin("FXCADUSD", oProveedorServicio)
                }
                // Vaciamos - Objeto
                oObjectTiposCambioArray.push(oObjectTipoCambio);

                // Seteamos - Elemento - FXCADUSD
                oObjectTipoCambio = {
                    TIME_PERIOD: oTimePeriod,
                    OBS_VALUE: (1 / parseFloat(oTiposCambioJSON.observations[i].FXCADUSD.v)).toFixed(oDecimalesRedondeoBaseInvertido),
                    OBS_VALUE_WS: parseFloat(oTiposCambioJSON.observations[i].FXCADUSD.v).toFixed(oDecimalesRedondeoBaseInvertido),
                    TYPECOIN: __getTypeCoin("FXUSDCAD", oProveedorServicio)
                }
                // Vaciamos - Objeto
                oObjectTiposCambioArray.push(oObjectTipoCambio);
            }

            // Convertimos - Objeto a JSON
            oObjectTiposCambioJSON = JSON.parse(JSON.stringify(oObjectTiposCambioArray));
        }

        // Retornamos - Valor
        return oObjectTiposCambioJSON;
    } catch (e) {
        // Almacenamos - Mensaje - Error
        oMessageError = e.message;
        // Almacenamos - Mensaje - Excepción
        oMessageException = '[Función: __getObtTiposCambio()] Ocurrio un error al momento de obtener los tipos del cambio: ' + e.message;
        // Lanzamos - Excepción
        throw oMessageException;
    }
}

function nlapiRequestURL(url,Parametros,Encabezado)
{
    var req = new XMLHttpRequest();
    var res;
    Encabezado = "application/json;charset=UTF-8";
    let urls = new URL(url);
    urls.searchParams.set('token', '6d3bb969429ac7d4c13ab74bdbdde844eb557d071733e3e117ba4973d30d26c5');
    req.open('GET', urls, true);
    req.withCredentials = true;
    req.setRequestHeader("Content-Type", "application/json");
    req.send();
    if (req.status == 200)
    {
      res = req.responseText;
    }
    //return dump(req.responseText);
    return res;
}