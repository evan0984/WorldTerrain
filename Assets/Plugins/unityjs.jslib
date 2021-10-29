mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Sent from Thought World Unity!");
  },

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  },

  BroadcastThoughtID: function (str) {
    parent.postMessage(Pointer_stringify(str),"*");
  },

});