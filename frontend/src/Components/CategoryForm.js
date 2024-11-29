import React, { useState, useEffect } from "react";
import "./css/CategoryForm.css";

function CategoryForm({ setCategories }) {
  const [fetchedCategories, setFetchedCategories] = useState([]);
  const [selectedCategories, setSelectedCategories] = useState([]);
  const [isOpen, setIsOpen] = useState(false);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Events/Categories`);
        if (!response.ok) {
          throw new Error('Błąd podczas pobierania kategorii.');
        }
        const categories = await response.json();
        setFetchedCategories(categories);
      } catch (err) {
        console.error(err);
      }
    };
    fetchCategories();
  }, []);

  const handleCategoryForm = (e) => {
    const value = e.target.value;
    let updatedSelectedCategories;
    if (e.target.checked) {
      updatedSelectedCategories = [...selectedCategories, value];
    } else {
      updatedSelectedCategories = selectedCategories.filter((category) => category !== value);
    }
    setSelectedCategories(updatedSelectedCategories);
    setCategories(updatedSelectedCategories);
  };

  const toggleSidebar = () => {
    setIsOpen(!isOpen);
  };

  return (
    <div className={`category-form ${isOpen ? 'open' : 'closed'}`}>
      <button 
        className="category-form__toggle"
        onClick={toggleSidebar}
        aria-label={isOpen ? 'Zamknij panel kategorii' : 'Otwórz panel kategorii'}
      >
        <svg 
          className="category-form__toggle-icon" 
          viewBox="0 0 24 24" 
          width="32" 
          height="32"
        >
          <path 
            fill="currentColor" 
            d={isOpen 
              ? "M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z" 
              : "M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z" 
            }
          />
        </svg>
      </button>
      <div className="category-form__content">
        <h3 className="category-form__title">Wybierz kategorię wydarzenia</h3>
        <div className="category-form__checkboxes">
          {fetchedCategories.map((category) => (
            <div key={category} className="category-form__checkbox">
              <input
                type="checkbox"
                value={category}
                onChange={handleCategoryForm}
                checked={selectedCategories.includes(category)}
                id={`category-${category}`}
                className="category-form__input"
              />
              <label htmlFor={`category-${category}`} className="category-form__label">
                {category}
              </label>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

export default CategoryForm;
