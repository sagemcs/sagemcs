//PORTAL DE PROVEDORES T|SYS|
//25 FEBRERO DEL 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.
//ACTUALIZADO POR : LUIS ANGEL GARCIA
var table;
var facturas_seleccionas = {};
$(document).ready(function () {
    hide_combo_proveedores(FacturasWebService.get_path());
    var url_list = FacturasWebService.get_path() + "/listar_sin_contrarecibo";

    function crearTabla(data) {
        
        var table = [
            '<table class="datatable table  dataTable">',
            '<thead>',
            '<tr><th colspan="6" style="color:brown;background-color: lightgrey;">Notas de cr&eacute;dito/d&eacute;bito</th></tr>',
            '<tr>',
            '<th>Folio</th>',
            '<th>Serie</th>',
            '<th>Fecha</th>',
            '<th>Subtotal</th>',
            '<th>Total</th>',
            '<th>Tipo</th>',
            '</tr>',
            '</thead>',
            '<tbody>'];

        data.Notas.forEach(function (row) {
            var tipo = row.Tipo == 'CM' ? 'Cr&eacute;dito' : 'D&eacute;bito';
            var color = row.Tipo == 'CM' ? 'green' : 'red';
            table.push('<tr>');
            table.push('<td>' + row.Folio + '</td>');
            table.push('<td>' + row.Serie + '</td>');
            table.push('<td>' + row.Fecha + '</td>');
            table.push('<td>' + row.Subtotal + '</td>');
            table.push('<td>' + row.Total + '</td>');
            table.push('<td style="color:' + color+'">' + tipo + '</td>');
            table.push('</tr>');
        });
        table.push('</tbody>');
        table.push('</table>');

        return table.join('');
    }   
    table = $('#list').DataTable({
        language: window.datatableLang,
        "ajax": {
            "url": url_list,
            "type": "POST",
            "beforeSend": function (xhr) {
                xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
            },
            "draw": 1,
            "data": function (data) {
                data.order_col = data.order[0]['column'];
                data.order_dir = data.order[0]['dir'];
                data.VendID = $('#MainContent_comboProveedores').val();
                data.Folio = '';
                data.Serie = '';
                data.Fecha = $('#MainContent_inputFecha').val();
                data.FechaAP = $('#MainContent_inputAproba').val();
                data.Total = '';
                data.UUID = '';
                data.Status = '';
            }
        },
        searching: false,
        'aLengthMenu':[[10, 25, 50, 100, -1],[10, 25, 50, 100, "All"]],
        // 'aLengthMenu':[[2, 5, 10, 100, -1],[2, 5, 10, 100, "All"]],
        // "bStateSave": true, right
        "processing": true,
        "serverSide": true,
        "stateSave": true,
        "createdRow": function (row, data, dataIndex) {
            $(row).attr('data-id', data.UUID);         
            $(row).attr('data-fecha', data.Fecha);
            $(row).attr('data-subtotal', data.Subtotal);
            $(row).addClass(data.UUID);
        },
        "columns": [
            { "data": "UUID" , 'className': "centrar-data", "orderable": false},
            { "data": "Compania" , 'className': "centrar-data text_align_left"},
            { "data": "Folio" , 'className': "centrar-data text_align_left"},
            { "data": "Serie", 'className': "centrar-data" },
            { "data": "Fecha_Recepcion", 'className': "centrar-data" },
            { "data": "Fecha_Aprobacion", 'className': "centrar-data" },
            { "data": "Proveedor_Nombre", 'className': "centrar-data" },
            { "data": "Subtotal", 'className': "centrar-data text_align_right" },
            { "data": "Retenciones", 'className': "centrar-data text_align_right" },
            { "data": "Traslados", 'className': "centrar-data text_align_right" },
            { "data": "Total", 'className': "centrar-data text_align_right" },
            { "data": "Notas", 'className': "centrar-data details-control", "orderable": false },
            { "data": "Importe", 'className': "cetrar-data text_align_right", "orderable": false },
        ],

        "columnDefs": [ {
            'targets': 0,
            'checkboxes': {
                'selectRow': true
            }
        }, {
            "targets": -2,
            "data": null,
            "render": function (data, type, row) {
                if (!row.Notas) {
                    return "-";
                }
                var Notas = row.Notas;
                
                if (Notas.length == 0) {
                    return "-";
                }

                var valorNota = 0;
                Notas.forEach(function (nota) {
                    if (nota.Tipo == 'DM') {
                        valorNota += parseFloat(nota.Total);
                    }
                    if (nota.Tipo == 'CM') {
                        valorNota -= parseFloat(nota.Total);
                    }
                });
                valorNota = valorNota.toFixed(2);
                var encode_notas = JSON.stringify(Notas);
                encode_notas = btoa(encode_notas);
                var render = "";
                //                render += '<a class="btn btn-xs btn-primary" title="Ver notas" data-tootle="tooltip" data-uuid="' + row.UUID + '" data-notas="' + row.Notas + '" onclick="mostrarNotas(this);"><i class="glyphicon glyphicon-plus" ></i >Notas</a>';
                if (valorNota >= 0) {
                    render += '<a class="btn btn-xs btn-primary" title="Ver notas" data-tootle="tooltip" data-uuid="' + row.UUID + '" data-notas="' + encode_notas + '" onclick="mostrarNotas(this);">' + valorNota+'</a>';

                } else {
                    render += '<a class="btn btn-xs btn-primary" title="Ver notas" data-tootle="tooltip" data-uuid="' + row.UUID + '" data-notas="' + encode_notas + '" onclick="mostrarNotas(this);">' + valorNota + '</a>';

                }
                
                return render;
               
            }
            }, {
                "targets": -1,
                "data": null,
                "render": function (data, type, row) {
                    //if (!row.Notas) {
                    //    return "-";
                    //}
                    //data.columns(7).hide;
                    var Notas = row.Notas;

                    //if (Notas.length == 0) {
                    //    return "-";
                    //}

                    var valorNota = 0;
                    Notas.forEach(function (nota) {
                        if (nota.Tipo == 'DM') {
                            valorNota += parseFloat(nota.Total);
                        }
                        if (nota.Tipo == 'CM') {
                            valorNota -= parseFloat(nota.Total);
                        }
                    });

                    var str2 = row.Total.substr(0, 1);
                    var text = row.Total;
                    var str3 = text.replace('$', '');
                    var str4 = str3.replace(',', '');
                    var str5 = parseFloat(str3).toFixed(2)
                    //var str6 = str2.concat(str5);
                    return row.Total;
                }
            }
        ],
        'select': {
            selector:'td:not(:last-child)',
            'style': 'multi'
        },
        'order': [[1, 'asc']]
    });

    //-----detail -test---
    // Array to track the ids of the details displayed rows
    var detailRows = [];
    $('#list tbody').on('click', 'tr td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);
        var idx = $.inArray(tr.attr('id'), detailRows);

        if (row.child.isShown()) {
            tr.removeClass('details');
            row.child.hide();

            // Remove from the 'open' array
            detailRows.splice(idx, 1);
        }
        else {
            tr.addClass('details');
            row.child(crearTabla(row.data())).show();

            // Add to the 'open' array
            if (idx === -1) {
                detailRows.push(tr.attr('id'));
            }
        }
    });

    // On each draw, loop over the `detailRows` array and show any child rows
    table.on('draw', function () {
        $.each(detailRows, function (i, id) {
            $('#' + id + ' td.details-control').trigger('click');
        });
    });

    //-----fin detail--
   

    $('.buscar').click(function (e) {
        e.preventDefault();
        //if (!$("#MainContent_comboProveedores").val()) {
        //    showToastError("Seleccione un proveedor");
        //    return false;
        //}
        //if (!$("#MainContent_inputFecha").val()) {
        //    showToastError("Seleccione una fecha");
        //    return false;
        //}
        table.ajax.reload(  );
    });
    $('.limpiar').click(function (e) {
        e.preventDefault();
        $('#MainContent_inputFecha').val(''); 
        $('#MainContent_inputAproba').val('');
        $('#MainContent_inputRFC').val('');
        $("#MainContent_comboProveedores").val($("#MainContent_comboProveedores option:first").val()).change();
        table.ajax.reload(  );
    });


    function getSelected() {
        //var rows_selected = table.column(0).checkboxes.selected();
        var ids = [];
        $.each(facturas_seleccionas, function (prop, data) {
            ids.push(prop);
        });

        return ids;
    }

    function getSelectedFechas() {
        //var rows_selected = table.column(0).checkboxes.selected();
        var fechas = [];
        $.each(facturas_seleccionas, function (prop, data) {
            fechas.push(data.Fecha_Recepcion);
        });
        return fechas;
    }

    function getSelectedMonedas() {
        //var rows_selected = table.column(0).checkboxes.selected();
        var monedas = [];
        $.each(facturas_seleccionas, function (prop, data) {
            monedas.push(data.Moneda);
        });
        return monedas;
    }

    function verificarFacturasFechas() {
        var fechas = getSelectedFechas();
        var result = true;
        var fecha0 = fechas[0];
        fechas.forEach(function (fecha) {
            if (fecha != fecha0) {
                result = false; return;
            }
        });
        return result;
    }

    function verificarFacturasMonedas() {
        var monedas = getSelectedMonedas();
        var result = true;
        var moneda0 = monedas[0];
        monedas.forEach(function (moneda) {
            if (moneda != moneda0) {
                result = false; return;
            }
        });
        return result;
    }

    $('form').submit(function () {

        var ids = getSelected();
        if (ids.length == 0) {
            showToastError("Seleccione las facturas para generar el contrarecibo.");
            return false;
        }
        if (verificarFacturasFechas() == false) {
            showToastError("Las facturas seleccionadas deben ser de la misma fecha.");
            return false;
        }
        if (verificarFacturasMonedas() == false) {
            showToastError("Las facturas seleccionadas deben ser de la misma moneda.");
            return false;
        }
        showToastConfirmation("Desea crear el contrarecibo?", "", function () {
            crearContrarecibo(ids); return false;
        }, function () {

        });
        return false;
        var json_ids = JSON.stringify(ids);
        $('#MainContent_facturas_seleccionadas').val(json_ids);
        return true;
    });

    function crearContrarecibo(ids) {
        var url_generar_contrarecibo = ContrarecibosWebService.get_path() + "/generar";
        blockUI({
            boxed: true,
            message: 'Generando contrarecibo...'
        });
        
            $.ajax({
                type: "POST",
                beforeSend: function (xhr) {   //Include the bearer token in header
                    xhr.setRequestHeader("Authorization", 'Bearer ' + GetJWT());
                },
                url: url_generar_contrarecibo,
                dataType: "json",
                data: {
                    ids: JSON.stringify(ids)
                },
                success: function (respuesta) {
                    unblockUI();
                    if (respuesta.success == true) {
                        facturas_seleccionas = {};
                        table.ajax.reload();
                        var contrarecibo_id = respuesta.id;
                        redirectToReport(contrarecibo_id);
                    } else {
                        showToastError(respuesta.msg ? respuesta.msg : "No se pudo crear el contrarecibo.");
                    }
                },
                error: function (respuesta) {
                    unblockUI();
                    showToastError("No se pudo crear el contrarecibo.");
                }
            });
    }

    function redirectToReport(id) {
        var url_report = "/Logged/Reports/Contrarecibo?id=" + id;
        window.open(url_report, '_blank');
    }
    table.on('select', function (ev, el, o, index) {
        var data = el.row(index).data();
        facturas_seleccionas[data.UUID] = data;
        actualizarImporte();
    });

    table.on('deselect', function (ev, el, o, index) {
        var data = el.row(index).data();
        delete facturas_seleccionas[data.UUID];
        actualizarImporte();
    });

    function calcularImporteTotal() {
        var importeTotal = 0;
        var uno = 0;
        var dos = 0;
        $.each(facturas_seleccionas, function (key, data) {
            var valorNota = 0;
            data.Notas.forEach(function (nota) {
                if (nota.Tipo == 'DM') {
                    valorNota += parseFloat(nota.Total);
                }
                if (nota.Tipo == 'CM') {
                    valorNota -= parseFloat(nota.Total);
                }
            });

            var cu = data.Total;
            var cin = cu.replace('$', '').replace(',', '');
            var seis = parseFloat(cin);

            //uno= parseFloat(data.Total.replace('$', ''))
            //dos = parseFloat(uno.toString().replace(',', ''));
            //importeTotal += dos + valorNota;
            importeTotal += seis + valorNota;
            tres = parseFloat(importeTotal).toFixed(2);
        });
        return importeTotal.toFixed(2);
    }

    function formatNumber(num) {
        if (!num || num == 'NaN') return '-';
        if (num == 'Infinity') return '&#x221e;';
        num = num.toString().replace(/\$|\,/g, '');
        if (isNaN(num))
            num = "0";
        sign = (num == (num = Math.abs(num)));
        num = Math.floor(num * 100 + 0.50000000001);
        cents = num % 100;
        num = Math.floor(num / 100).toString();
        if (cents < 10)
            cents = "0" + cents;
        for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
            num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
        return (((sign) ? '' : '-') + num + '.' + cents);
    }

    function actualizarImporte() {
        var importe = calcularImporteTotal();
        var Total = formatNumber(importe);
        $('#importe-total').html(Total);
    }

    

} );

function mostrarNotas(el) {
    return false;
    //var data = $(el).data();
    var notas = $(el).data('notas');
    notas = atob(notas);
    notas = JSON.parse(notas);
    //var nTr = $(el).parents('tr')[0];
    var tableHtml = crearTabla(notas);
    $('#modal-content-table').html(tableHtml);
    $('#modalDetallesNota').modal('show');
    return;
    console.log(table);
    if (table.fnIsOpen(nTr)) {
        /* This row is already open - close it */
        $(el).addClass("row-details-close").removeClass("row-details-open");
        $(el).find('i').addClass("glyphicon glyphicon-plus").removeClass("glyphicon glyphicon-minus");
        oTable.fnClose(nTr);
    }
    else {
        $(el).addClass("row-details-open").removeClass("row-details-close");
        $(el).find('i').addClass("glyphicon glyphicon-minus").removeClass("glyphicon glyphicon-plus");
        table.fnOpen(nTr, fnFormatDetails(table, nTr, { data: notas }), 'table-details');
        $('.table-details').attr('colspan', '12');
    }
}

function fnFormatDetails(oTable, nTr, options) {

    var sOut = '<table class="table">';
    sOut += '<tr class="odd text-center"><td style="color:black">' + options.data + '</td></tr>';
    sOut += '</table>';
    return sOut;
}

