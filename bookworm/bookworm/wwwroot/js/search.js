const radioButtons = document.querySelectorAll('input[type="radio"]');
const selectedOptionText = document.getElementById('selectedOptionText');

// Initialize with the default selected value
const initialSelectedValue = document.querySelector('input[name="options"]:checked + label').textContent;
selectedOptionText.textContent = initialSelectedValue;

// Listen for changes in radio button selection
radioButtons.forEach((radio) => {
  radio.addEventListener('change', () => {
    const selectedValue = document.querySelector('input[name="options"]:checked + label').textContent;
    selectedOptionText.textContent = selectedValue;
  });
});
