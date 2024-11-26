import React, { useState, useEffect } from "react";
import "./css/CategoryForm.css";

function CategoryForm({ setCategories }) {
  const [fetchedCategories, setFetchedCategories] = useState([]);
  const [selectedCategories, setSelectedCategories] = useState([]);

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

  return (
    <div className="category-form">
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
  );
}

export default CategoryForm;
