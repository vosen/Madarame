$(document).ready(function () {
    var title_add_button = '<button class="btn btn-success button-seen"><i class="icon-white icon-plus"></i> Seen it</button>';
    var title_del_float = '<span class="float-seen-del"><select name="rating"><option>10</option><option>9</option><option>8</option><option>7</option><option>6</option><option>5</option><option>4</option><option>3</option><option>2</option><option>1</option></select></td><td><button class="btn btn-danger"><i class="icon-white icon-minus"></i></button></td></tr></span>';
    var replace_rating_with_seen = function (event) {
        var add_float = $(title_add_button);
        add_float.click(replace_seen_with_rating);
        var $target = $(event.currentTarget);
        // enable hidden
        $target.closest('td').find('input').attr('disabled', 'disabled');
        // insert del float
        $target.closest('span').replaceWith(add_float);
        event.preventDefault();
    };
    var replace_seen_with_rating = function (event) {
        var del_float = $(title_del_float);
        del_float.find('button').click(replace_rating_with_seen);
        var $target = $(event.currentTarget);
        // disable hidden
        $target.closest('td').find('input').removeAttr('disabled');
        // insert button float
        $target.replaceWith(del_float);
        event.preventDefault();
    };
    $('.table-titles td span').append(title_add_button);
    $('.button-seen').click(replace_seen_with_rating);
});