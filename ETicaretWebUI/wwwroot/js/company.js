var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblCompany').DataTable({
        "ajax": { url: '/admin/company/getall' },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'streetAddress', "width": "15%" },
            { data: 'city', "width": "15%" },
            { data: 'state', "width": "10%" },
            { data: 'postalCode', "width": "10%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: "id",
                "render": function (data) {
                    return `
                        <div class="row">
                            <div class="col-6">
                                    <td>
                                        <a href="/admin/company/edit?id=${data}" class="btn btn-sm btn-info w-100"><i class="bi bi-pencil-square me-1"></i> Ürün Düzenle</a>
                                    </td>
                                    </div>
                                     <div class="col-6">
                                    <td>
                                        <a onClick=Delete('/admin/company/deleteTableCompany/${data}') class="btn btn-sm btn-danger w-100"><i class="bi bi-trash me-1"></i>Ürün Kaldır</a>

                                    </td>
                                </div>
                            </div>`


                },
                "width": "20%"

            }


        ]
    });
}

//SweetAlert custom confirm eklendisini tanımla toastr ile birlikte 
function Delete(url) {
    Swal.fire({
        title: "Silmek istediğinize emin misiniz?",
        text: "Bu işlem geri alınamaz!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Evet, ürünü kaldır!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}