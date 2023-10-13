
function SuccessAutoCloseAlert(title)
{
    Swal.fire({
        icon: 'success',
        title: title,
        // text: 'The status has been changed successfully.',
        timer: 1050, // Specify the duration in milliseconds
        timerProgressBar: true,

    });

}

function ErrorAutoCloseAlert(title)
{
    Swal.fire({
        icon: 'error',
        title: title,
        // text: 'The status has been changed successfully.',
        timer: 1050, // Specify the duration in milliseconds
        timerProgressBar: true,

    });

}