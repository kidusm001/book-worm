const labels = document.querySelectorAll('.form-control label');
labels.forEach((label) => {
  label.innerHTML = label.innerText
    .split('')
    .map((letter, idx) => `<span style ="transition-delay:${idx * 50}ms">${letter}</span>`)
    .join('');
});

// Function to check inputs on page load
function checkInputsOnLoad() {
  const inputs = document.querySelectorAll('.login-detail');
  inputs.forEach((input) => {
    if (input.value.trim() !== '') {
      // Add class if input has value
      input.classList.add('has-value');
    } else {
        // Remove the class if the input is empty
        input.classList.remove('has-value');
    }
  });
}

// Call the function when the page is loaded
document.addEventListener('DOMContentLoaded', checkInputsOnLoad);

const inputs = document.querySelectorAll('.login-detail');
inputs.forEach((input) => {
  input.addEventListener('input', () => {
    if (input.value.trim() !== '') {
      // Add a class when something is written inside the input
      input.classList.add('has-value');
    } else {
      // Remove the class if the input is empty
      input.classList.remove('has-value');
    }
  });
});
