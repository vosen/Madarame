$(document).ready(function () {
    var row_counter = 0;
    var title_row = '<tr><td><input class="input typeahead" name="title" type="text" data-provide="typeahead"><input name="id" type="hidden"></td><td><select name="rating"><option>10</option><option>9</option><option>8</option><option>7</option><option>6</option><option>5</option><option>4</option><option>3</option><option>2</option><option>1</option></select></td><td><button class="btn btn-danger button-del disabled" disabled="disabled"><i class="icon-white icon-minus"></i></button></td></tr>';
    var typeahead_handler = {
        source: function (typeahead, query) {
            $.ajax({
                url: "/api/json/asynconeway/SearchQuery",
                data: { term: query },
                success: function (data) {
                    typeahead.process(data)
                }
            });
        },
        matcher: function (item) {
            return true;
        },
        onselect: function (item) {
            this.$element.siblings().val(item.id);
        },
        property: "value"
    };
    var del_handler = function (event) {
        row_counter--;
        $(this).unbind('click').parents('tr').remove();
        if (row_counter <= 1) {
            $('#title-list .button-del').addClass('disabled').attr('disabled', 'disabled');
        }
        event.preventDefault();
    };
    var add_handler = function (event) {
        row_counter++;
        var title_body = $('#title-list tbody');
        title_body.append(title_row);
        var typeahead = title_body.find('tr:last .typeahead').typeahead(typeahead_handler);
        $('.typeahead').on('change', function () {
            $(this).siblings().val(undefined);
        });
        title_body.find('tr:last .button-del').click(del_handler);
        if (row_counter > 1) {
            $('#title-list .button-del').removeClass('disabled').removeAttr('disabled');
        }
        if(event !== undefined)
            event.preventDefault();
    };
    add_handler();
    $('.button-add').click(add_handler);
});