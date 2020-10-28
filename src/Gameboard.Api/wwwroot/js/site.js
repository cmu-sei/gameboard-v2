// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

function toggle(t, id) {
    t.style.display = 'none';
    var e = document.getElementById(id);
    e.style.display = e.style.display === 'block' ? 'none' : 'block';
}
