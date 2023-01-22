const dropdowns = document.querySelectorAll(".dropdown");

dropdowns.forEach(dropdown => {
   const select = dropdown.querySelector(".dropdown__select");
   const options = dropdown.querySelectorAll(".dropdown__list li");
   const selected = dropdown.querySelector(".dropdown__selected");

   select.addEventListener("click", () => {
      dropdown.classList.toggle("active");
      document.addEventListener("click", closeByOuterClick);
   });

   options.forEach(option => {
      option.addEventListener("click", () => {
          selected.innerText = option.innerText;
          dropdown.classList.remove("active");
          options.forEach(option => {
              option.classList.remove("dropdown__active");
          })
          option.classList.add("dropdown__active");
          document.removeEventListener("click", closeByOuterClick);
      }) ;
   });

    function closeByOuterClick(e) {
        if (e.target !== dropdown && !dropdown.contains(e.target)) {
            if (dropdown.classList.contains("active")) {
                dropdown.classList.remove("active");
                document.removeEventListener("click", closeByOuterClick);
            }
        }
    }
});