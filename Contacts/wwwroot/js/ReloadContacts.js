var connection = new signalR.HubConnectionBuilder().withUrl("/ReloadContactGrid").build();

// Start the connection
connection.start();

connection.on("DataUpdated", function () {
    // When data is updated, reload the DataTable for all clients
    LoadData();
});
function LoadData() {
    if ($.fn.DataTable.isDataTable('#contacts')) {
        $('#contacts').DataTable().destroy();
    }
    $('#contacts').dataTable({
        "serverSide": true,
        "searching": true,
        "ajax": {
            "url": "/api/contacts",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
           // "visible": false,
            "searchable": false
        }],
        "pageLength": 5,
        "lengthMenu": [5, 10, 15, 50], // Specify the available page lengths

        "columns": [
            { "data": "id", "name": "Id", "autowidth": true },
            { "data": "name", "name": "Name", "autowidth": true },
            { "data": "phone", "name": "Phone", "autowidth": true },
            { "data": "address", "name": "Address", "autowidth": true },
            { "data": "notes", "name": "Notes", "autowidth": true },
            {

                "render": function (data, type, row)
                {
                   // return "<a href='#' class='btn btn-success' onclick='EditContact(" + JSON.stringify(row) + ");' > Edit </a>"
                    return '<a href="#" class="btn btn-success edit-row">Edit</a><a href="#" class="btn btn-primary save-row" style="display: none;">Save</a>';

                },
                "orderable": false
            },
            {
                "render": function (data, type, row)
                {
                    return '<a href="#" class="btn btn-danger" onclick=DeleteContactConfirmation("' + row.id + '"); > Delete </a>'
                   // return '<a href="#" class="btn btn-primary save-row" style="display: none;">Save</a>';

                },
                "orderable": false
            },

        ]
    });

}



$('#contacts').on('click', '.edit-row', function () {
    var row = $(this).closest('tr');
    row.find('.edit-row').hide();
    row.find('.save-row').show();

    row.find('td:not(:first-child)').attr('contenteditable', 'true');
    row.find('td:not(:eq(0), :eq(5), :eq(6))').css('border', '1px solid #000');
});


$('#contacts').on('click', '.save-row', function () {
    var row = $(this).closest('tr');
    row.find('.edit-row').show();
    row.find('.save-row').hide();

    row.find('td:not(:first-child)').attr('contenteditable', 'false');
    debugger;




    // Extract the data from the row and update it using AJAX
    var id = row.find('td:eq(0)').text(); // Get the ID
    var name = row.find('td:eq(1)').text(); // Get the Name
    var phone = row.find('td:eq(2)').text(); // Get the Phone
    var address = row.find('td:eq(3)').text(); // Get the Address
    var notes = row.find('td:eq(4)').text(); // Get the Notes

    // Prepare the data for updating using AJAX
    var data = {
        Id: id,
        Name: name,
        Phone: phone,
        Address: address,
        Notes: notes
    };
    // Send the data to the server for saving
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        type: "POST",
        url: '/api/Contacts/AddEditContact',
        data: JSON.stringify(data),
        contentType: 'application/json',
        headers: {
            'RequestVerificationToken': token
        },
        success: function (result) {
            resetdata();
            SuccessAutoCloseAlert('تم الحفظ بنجاح');
            // Notify clients using SignalR
            connection.invoke("DataChanged").catch(function (err) {
                console.error(err.toString());
            });
        },
        error: function (xhr, status, error) {
            ErrorAutoCloseAlert('لم يتم الحفظ');
        }
    });
});

