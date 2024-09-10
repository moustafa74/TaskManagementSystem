$(document).ready(function () {
    // Initially show the login form and hide the register form
    $('#login-form-container').show();
    $('#register-form-container').hide();
    $('#success-message-register').hide();
    $('#success-message').hide();

    // Function to check if the token is expired
    function isTokenExpired(token) {
        if (!token) return true; 

        // Decode the token payload 
        const payload = JSON.parse(atob(token.split('.')[1])); 
        console.log(payload);
        const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
        console.log(currentTime);
        return payload.exp < currentTime;
    }

    // Check if the user is logged in by checking the token
    const token = localStorage.getItem('authToken');
    console.log('Token:', token);
    if (!token || isTokenExpired(token)) {
        $('#dashboard-link, #tasks-link, #teams-link, #logout-link').hide();
        $('#login-link').show();
        $('#success-message').hide();
        localStorage.removeItem('authToken');
        localStorage.removeItem('UserID');
    } else {
        $('#dashboard-link, #tasks-link, #teams-link, #logout-link').show();
        $('#login-link').hide();
        $('#auth-form').hide();
        $('#success-message').show();
    }

    $('#show-login').click(function () {
        $('#login-form-container').show();
        $('#register-form-container').hide();
        $('#show-login').addClass('active');
        $('#show-register').removeClass('active');
    });

    $('#show-register').click(function () {
        $('#login-form-container').hide();
        $('#register-form-container').show();
        $('#show-login').removeClass('active');
        $('#show-register').addClass('active');
    });

    // Handle login form submission
    $('#login-form').submit(function (event) {
        event.preventDefault();
        const email = $('#email-login').val();
        const password = $('#password-login').val();

        $.ajax({
            url: '/api/login',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ UserName: email, Password: password }),
            success: function (response) {
                    localStorage.setItem('authToken', response.token);
                    localStorage.setItem('UserID', response.user_ID);
                    $('#dashboard-link, #tasks-link, #teams-link, #logout-link').show();
                    $('#login-link').hide();
                    $('#auth-form').hide();
                    $('#success-message').show();
                    //$('#login-link').hide();
                    //window.location.href = 'tasks.html'; // Redirect to task page after login
            },
            error: function (error) {
                const errors = error.responseJSON.error;

                let errorMessage = '';

                if (errors && Array.isArray(errors)) {
                    errors.forEach((err) => {
                        errorMessage += `${err} `;
                    });
                } else {
                    errorMessage = 'Error registering user. Please try again.';
                }

                $('#error-message').html(errorMessage);
            }
        });
    });

    // Handle register form submission
    $('#register-form').submit(function (event) {
        event.preventDefault();
        const FullName = $('#FullName').val();
        const email = $('#email-register').val();
        const password = $('#password-register').val();
        const confirmPassword = $('#confirmPassword').val();

        if (password !== confirmPassword) {
            $('#error-messagereg').text('Passwords do not match!');
            return;
        }

        $.ajax({
            url: '/api/Register',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                FullName: FullName,
                Email: email,
                Password: password
            }),
            success: function (response) {
                $('#auth-form').hide();
                $('#success-message-register').show();

                ////$('#go-to-login').click(function () {
                ////    $('#show-login').click(); 
                //});
            },
            error: function (error) {
                const errors = error.responseJSON.error; 

                let errorMessage = '';

                if (errors && Array.isArray(errors)) {
                    errors.forEach((err) => {
                        errorMessage += `${err} `;
                    });
                } else {
                    errorMessage = 'Error registering user. Please try again.'; 
                }

                $('#error-messagereg').html(errorMessage);
            }
        });
    });


    // Logout function
    $('#logout-link').click(function (event) {
        event.preventDefault(); // Prevent the immediate navigation

        // Remove token from localStorage
        localStorage.removeItem('authToken');

        // Hide and show relevant links after logging out
        $('#dashboard-link, #tasks-link, #teams-link, #logout-link').hide();
        $('#login-link').show();

        // Redirect to login page after logout
        window.location.href = 'index.html';
    });
    // Handle Recovery Form submission on the Recovery page
    $('#recovery-form').submit(function (event) {
        event.preventDefault();

        const email = $('#email-recovery').val();

        $.ajax({
            url: '/api/login/ForgetPassword',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                Email: email,
            }),
            success: function (response) {
                //const user_id = response.user_id;
                //$('#user_id').val(user_id);
                $('#error-message-recovery').text('Recovery email has been sent successfully.');
                $('#error-message-recovery').css('color', 'green');
            },
            error: function (error) {
                const errors = error.responseJSON.error;
                let errorMessage = '';

                if (errors && Array.isArray(errors)) {
                    errors.forEach((err) => {
                        errorMessage += `${err} `;
                    });
                } else {
                    errorMessage = 'An error occurred. Please try again.'; 
                }
                $('#error-message-recovery').text(errorMessage);
                $('#error-message-recovery').css('color', 'red');
            }
        });
    });
    // Handle Reset Passowrd Form submission on Reset Password Page
    $('#reset-password-form').submit(function (event) {
        event.preventDefault();

        const newPassword = $('#newPassword').val();
        const confirmNewPassword = $('#confirmNewPassword').val();
        const token = new URLSearchParams(window.location.search).get('token'); 
        const userId = new URLSearchParams(window.location.search).get('userId');

        if (newPassword !== confirmNewPassword) {
            $('#error-message-reset').text('Passwords do not match!');
            $('#error-message-reset').css('color', 'red');
            return;
        }

        $.ajax({
            url: '/api/Login/ResetPassword',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                token: token,
                NewPassword: newPassword,
                UserId: userId
            }),
            success: function (response) {
                $('#reset-password-form').hide();
                $('#success-message-reset').show();
            },
            error: function (error) {
                const errors = error.responseJSON.error;

                let errorMessage = '';

                if (errors && Array.isArray(errors)) {
                    errors.forEach((err) => {
                        errorMessage += `${err} `;
                    });
                } else {
                    errorMessage = 'Error resetting password. Please try again.';
                }

                $('#error-message-reset').html(errorMessage);
                $('#error-message-reset').css('color', 'red');
            }
        });
    });

});
