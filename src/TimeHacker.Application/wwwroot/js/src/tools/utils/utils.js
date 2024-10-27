function firstChild(parent, childSelector) {
    return parent.find(childSelector)[0]
}

function setValueToFirstChild(parent, childSelector, value) {
    $(firstChild(parent, childSelector)).val(value)
}