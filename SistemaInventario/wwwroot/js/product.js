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
            "url" : "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "serialNumber", },
            { "data": "description", },
            { "data": "category.name", },
            { "data": "brand.name", },
            {
                "data": "price", "className": "text-end",
                "render": function (data) {
                    return data.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');

                }
            },
            {
                "data": "status",
                "render": function (data) {
                    return data ? "Activo" : "Inactivo"
                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Product/Upsert/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="bi bi-pencil-square"></i>
                            </a>
                            <a onclick=Delete("/Admin/Product/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer">
                                <i class="bi bi-trash3-fill"></i>
                            </a>
                        </div>
                    `
                },
                "width": "20%"
            }
        ]
    });
}
function Delete(url) {
    swal({
        title: "¿Está seguro de eliminar el producto?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((erase) => {
        if (erase) {
            $.ajax({
                type: "POST",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function () {
                    toastr.error("Ocurrió un error al intentar eliminar el registro.");
                }
            });
        }
    });
} 
