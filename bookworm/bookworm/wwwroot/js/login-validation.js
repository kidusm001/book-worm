// login.js

$(document).ready(function () {
  $('#password').on('input', function () {
    var password = $(this).val();

    // Perform password validation checks
    var hasUpperCase = /[A-Z]/.test(password);
    var hasLowerCase = /[a-z]/.test(password);
    var hasNumbers = /\d/.test(password);
    var hasSymbols = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password);
    var isValidLength = password.length >= 8;

    // Update UI to indicate password strength
    if (isValidLength && hasUpperCase && hasLowerCase && hasNumbers && hasSymbols) {
      $('#passwordStrength').text('Password is strong').css('color', 'green');
    } else {
      $('#passwordStrength').text('Password is weak').css('color', 'red');
    }
  });
});
