const tabs = document.querySelectorAll(".footer__item");

tabs.forEach((tab) => {
   tab.addEventListener("click", (elem) => {
       tabs.forEach((tab) => {
           if (tab.classList.contains("active")) {
               tab.classList.remove("active");
           }
       });
       elem.currentTarget.classList.toggle("active");
       console.log(elem.currentTarget)
   });
});