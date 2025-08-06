var datatable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("aprobado")) {
        loadDataTable("GetOrderList?status=aprobado");
    }
    else {
        if (url.includes("completado")) {
            loadDataTable("GetOrderList?status=completado");
        }
        else {
            loadDataTable("GetOrderList?status=todas");
        }
    }

});

function loadDataTable(url) {
    datatable = $('#tblData').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ Registros por página",
            "zeroRecords": "Ningún registro",
            "info": "Mostrar page _PAGE_ de _PAGES_",
            "infoEmpty": "No hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax": {
            "url": "/Admin/Order/" + url
        },
        "columns": [
            { "data": "id" },
            { "data": "clientName" },
            { "data": "telephone" },
            { "data": "userApp.email" },
            { "data": "orderStatus" },
            {
                "data": "totalOrder", "className": "text-end",
                "render": function (data) {
                    var d = data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
                    return d;
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Order/Detail/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-ticket-detailed"></i>
                            </a>                           
                        </div>
                        `;
                }
            }
        ]
    });
}
