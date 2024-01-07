function ChangeImage(UploaImage, previewImg) {
    if (UploaImage.files && UploaImage.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(previewImg).attr('src', e.target.result);
        }
        reader.readAsDataURL(UploaImage.files[0]);
    }
}