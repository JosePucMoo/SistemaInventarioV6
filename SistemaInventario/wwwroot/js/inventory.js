let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "language": {
            "lengthMenu": "Mostrar _MENU_ registros por pagina",
            "zeroRecords": "Ningun registro",
            "info": "Mostrar página _PAGE_ de _PAGES_",
            "infoEmpty": "no hay registros",
            "infoFiltered": "(filtered from _MAX_ total registros)",
            "search": "Buscar",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax" : {
            "url" : "/Inventory/Inventory/GetAll"
        },
        "columns": [
            { "data": "store.name", },
            { "data": "product.description", },
            {
                "data": "product.cost", "className": "text-end",
                "render": function (data) {
                    return data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');

                }
            },
            { "data": "amount", "Classname": "text-end" },
        ]
    });
}