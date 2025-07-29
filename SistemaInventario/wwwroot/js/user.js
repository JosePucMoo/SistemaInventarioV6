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
            "url" : "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "email" },
            { "data": "name"},
            { "data": "lastname"},
            { "data": "phoneNumber"},
            { "data": "role"},
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    let today = new Date().getTime();
                    let block = new Date(data.lockoutEnd).getTime();
                    if (block > today) {
                        return `
                        <div class="text-center">
                            <a onclick=BlockUnblock("${data.id}") class="btn btn-danger text-white" style="cursor:pointer" width=150px>
                                <i class="bi bi-unlock-fill"></i> Desbloquear
                            </a>
                        </div>
                    `
                    } else { 
                        return `
                        <div class="text-center">
                            <a onclick=BlockUnblock("${data.id}") class="btn btn-success text-white" style="cursor:pointer" width=150px>
                                <i class="bi bi-lock-fill"></i> Bloquear
                            </a>
                        </div>
                    `
                    }
                },
                "width": "20%"
            }
        ]
    });
}
function BlockUnblock(id) {
    $.ajax({
        type: "POST",
        data: JSON.stringify(id),
        contentType: "application/json",
        url: "/Admin/User/BlockUnblock",
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
