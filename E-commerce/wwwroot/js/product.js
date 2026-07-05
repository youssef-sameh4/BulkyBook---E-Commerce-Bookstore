
var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    dataTable = $('#tblData').DataTable({

        ajax: {
            url: '/Admin/Products/GetAll'
        },

        columns: [
            { data: 'title', width: "25%" },
            { data: 'isbn', width: "15%" },
            { data: 'price', width: "15%" },
            { data: 'author', width: "15%" },

            {
                data: 'category.name',
                width: "10%",
                render: function (data) {
                    return `<span class="badge bg-secondary">${data}</span>`;
                }
            },

            {
                data: 'id',
                width: "15%",
                render: function (data) {

                    return `
                    <div class="d-flex justify-content-center gap-2">

                        <a href="/Admin/Products/Upsert?id=${data}" 
                           class="btn btn-sm btn-success">

                           <i class="bi bi-pencil-square"></i>
                           Edit
                        </a>

                        <a onClick="Delete('/Admin/Products/Delete/${data}')"
                           class="btn btn-sm btn-danger">

                           <i class="bi bi-trash-fill"></i>
                           Delete
                        </a>

                    </div>`;
                }
            }
        ]
    });
}

function Delete(url) {

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {

        if (result.isConfirmed) {

            $.ajax({
                url: url,
                type: 'DELETE',

                success: function (data) {

                    dataTable.ajax.reload();

                    Swal.fire({
                        title: 'Deleted!',
                        text: data.message,
                        icon: 'success'
                    });
                }
            });
        }
    });
}