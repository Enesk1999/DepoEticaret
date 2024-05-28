var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tbl').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            {
                data: 'imageUrl', "width": "15%",
                render: function (data, type, row) {
                    return '<img src="' + data + '" style="object-fit:cover; border-radius:2px; border:1px solid #bbaabb;" width="70" height="50">';
                },


            },
            { data: 'title', "width": "15%" },
            { data: 'author', "width": "15%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            {
                data: "id",
                "render": function (data) {
                    return `
                        <div class="row">
                            <div class="col-6">
                                    <td>
                                        <a href="/admin/product/edit?id=${data}" class="btn btn-sm btn-info w-100"><i class="bi bi-pencil-square me-1"></i> Ürün Düzenle</a>
                                    </td>
                                    </div>
                                     <div class="col-6">
                                    <td>
                                        <a onClick=Delete('/admin/product/deleteDatableProduct/${data}') class="btn btn-sm btn-danger w-100"><i class="bi bi-trash me-1"></i>Ürün Kaldır</a>

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