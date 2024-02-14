/* document.querySelectorAll('.file-drop-zone').forEach((dropZone) => {
  const input = dropZone.querySelector('input[type="file"]');
  const fileList = dropZone.querySelector('.file-list');

  dropZone.addEventListener('dragover', (event) => {
    event.preventDefault();
    dropZone.classList.add('drag-over');
  });

  dropZone.addEventListener('dragleave', () => {
    dropZone.classList.remove('drag-over');
  });

  dropZone.addEventListener('drop', (event) => {
    event.preventDefault();
    const files = event.dataTransfer.files;
    if (files.length > 0) {
      input.files = files;
      dropZone.classList.remove('drag-over');
      displayFileList(files, fileList);
    }
  });

  input.addEventListener('change', () => {
    const files = input.files;
    if (files.length > 0) {
      dropZone.classList.remove('drag-over');
      displayFileList(files, fileList);
    }
  });
});

function displayFileList(files, fileList) {
  fileList.innerHTML = '';
  files.forEach((file) => {
    const listItem = document.createElement('div');
    listItem.textContent = file.name;
    fileList.appendChild(listItem);
  });
} */
flatpickr('input[type=datetime]');

const dropAreas = document.getElementsByClassName('drop-area');
const dropImage = document.getElementById('drop-image-area');
const dropFile = document.getElementById('drop-file-area');
const imageInput = document.getElementById('bookImage');
const fileInput = document.getElementById('bookFile');
const imgView = document.getElementById('imgView');
const fileView = document.getElementById('fileView');
function handleFileDrop(e, input, view) {
  e.preventDefault();
  if (e.dataTransfer.files.length > 0) {
    const file = e.dataTransfer.files[0];
    const fileName = file.name;
    input.files = e.dataTransfer.files;
    view.innerHTML = `<i class="fa-regular fa-file"></i> <p>${fileName}</p>`;
    view.style.flexDirection = 'row';
  }
}

function setBackgroundImage(element, file) {
  const imgLink = URL.createObjectURL(file);
  element.style.backgroundImage = `url(${imgLink})`;
  element.style.backgroundSize = 'contain';
  element.style.backgroundPosition = 'center';
  element.style.backgroundRepeat = 'no-repeat';
  element.textContent = '';
}

imageInput.addEventListener('change', function () {
  let imgLink = URL.createObjectURL(this.files[0]);
  imgView.style.backgroundImage = `url(${imgLink})`;
  imgView.style.backgroundSize = 'contain';
  imgView.style.backgroundPosition = 'center';
  imgView.style.backgroundRepeat = 'no-repeat';
  imgView.textContent = '';
});
fileInput.addEventListener('change', function () {
  let fileName = this.files[0].name;
  fileView.innerHTML = `<i class="fa-regular fa-file"></i> <p>${fileName}</p>`;
  fileView.style.flexDirection = 'row';
});

// Event listeners for specific drop areas
dropImage.addEventListener('dragover', (e) => {
  e.preventDefault();
});
dropImage.addEventListener('drop', (e) => {
  e.preventDefault();
  if (e.dataTransfer.files.length > 0) {
    const file = e.dataTransfer.files[0];
    setBackgroundImage(imgView, file);
  }
});

dropFile.addEventListener('dragover', (e) => {
  e.preventDefault();
});
dropFile.addEventListener('drop', (e) => {
  handleFileDrop(e, fileInput, fileView);
});
