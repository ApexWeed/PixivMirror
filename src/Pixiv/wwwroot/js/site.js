// Write your Javascript code.
$(document).ready(function () {
    $('form').submit(function (event) {
        var formData = {
            'id': $('input[name=ID]').val(),
            'index': $('input[name=Index]').val()
        };

        $.ajax({
            type: 'post',
            url: $(this).attr("action"),
            data: formData,
            dataType: 'json',
            encode: true
        }).done(function (data) {
            console.log(data);

            if (data.success) {
                $('.help-block').prepend('<div class="alert alert-success"><input type="checkbox" checked="true" /><a class="illust" href="' + data.pixivURL + '">Pixiv Page</a> <a href="' + data.url + '" class="thumbnail"><span class="title">' + data.title + '</span> - <span class="author">' + data.author + '</span><img src="' + data.url + '" style="height:150px;"></a></div>');
            } else {
                $('.help-block').prepend('<div class="alert alert-danger">' + data.error + '</div>');
            }
        });

        event.preventDefault();
    });

    $('#gibebb').click(function (event) {
        var data = Array();

        $(".alert-success").each(function () {
            if ($("input:checkbox", this).is(":checked")) {
                data.push({
                    img: $("img", this).attr("src"),
                    pixiv: $(".illust", this).attr("href"),
                    title: $(".title", this).text(),
                    author: $(".author", this).text()
                });
            }
        });

        if (data.length > 0) {
            var textbox = $("#bbcode");
            textbox.val("");

            // Images up first.
            for (var floof in data) {
                textbox.val(textbox.val() + "[t]" + data[floof].img + "[/t]");
            }

            textbox.val(textbox.val() + "\n\nSource:");

            // Links
            var index = 0;
            for (var floof in data) {
                index++;
                //textbox.val(textbox.val() + ' [url="' + data[floof].pixiv + '"]' + index + '[/url]');
                textbox.val(textbox.val() + ' [url="' + data[floof].pixiv + '"]' + data[floof].title + ' - ' + data[floof].author + '[/url]');
            }
        }
    });
});
