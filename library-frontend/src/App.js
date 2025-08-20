import React, { useEffect, useState } from 'react';
import './App.css';
import QRScannerModal from './QRScannerModal';




const modalStyle = {
  position: 'fixed',
  top: '20%',
  left: '30%',
  width: '500px',
  backgroundColor: 'white',
  maxHeight: '50vh',              
  overflowY: 'auto',              
  padding: '20px',
  boxShadow: '0 0 10px rgba(0,0,0,0.3)',
  zIndex: 1000,
  borderRadius: '10px'
};

function AddBookModal({ onClose, onAdd, autoPlace = false }) {
  const [book, setBook] = useState({
    title: '', author: '', genre: '',
    height: '', width: '', type: 0,
    label: '', position: ''
  });
  const [bookshelves, setBookshelves] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/Bookshelves')
      .then(res => res.json())
      .then(setBookshelves)
      .catch(console.error);
  }, []);

  const handleAdd = () => {
    setError('');
    const url = autoPlace
      ? 'https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AutoPlaceBook'
      : 'https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AddBook';

    const requestBody = {
      ...book,
      height: +book.height,
      width: +book.width,
      position: book.position ? +book.position : null
    };

    fetch(url, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(requestBody)
    })
      .then(async res => {
        if (!res.ok) {
          const msg = await res.text();
          throw new Error(msg);
        }
        return res.json();
      })
      .then(() => {
        onAdd();
        onClose();
      })
      .catch(err => setError(err.message || 'Error occurred'));
  };

  return (
    <div style={modalStyle}>
      <h3>{autoPlace ? 'Auto Place Book' : 'Add Book (Manual Placement)'}</h3>

      <label>Title</label><br />
      <input value={book.title} onChange={e => setBook({ ...book, title: e.target.value })} /><br />

      <label>Author</label><br />
      <input value={book.author} onChange={e => setBook({ ...book, author: e.target.value })} /><br />

      <label>Genre</label><br />
      <input value={book.genre} onChange={e => setBook({ ...book, genre: e.target.value })} /><br />

      <label>Height (cm)</label><br />
      <input value={book.height} onChange={e => setBook({ ...book, height: e.target.value })} /><br />

      <label>Width (cm)</label><br />
      <input value={book.width} onChange={e => setBook({ ...book, width: e.target.value })} /><br />

      {!autoPlace && (
        <>
          <label>Bookshelf</label><br />
          <select value={book.label} onChange={e => setBook({ ...book, label: e.target.value })}>
            <option value="">Select Bookshelf</option>
            {bookshelves.map(bs => (
              <option key={bs.label} value={bs.label}>{bs.label}</option>
            ))}
          </select><br />

          <label>Shelf Position</label><br />
          <input value={book.position} onChange={e => setBook({ ...book, position: e.target.value })} /><br />
        </>
      )}

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <button onClick={handleAdd}>{autoPlace ? 'Auto Place Book' : 'Add Book'}</button>
      <button onClick={onClose} style={{ marginLeft: '10px' }}>Cancel</button>
    </div>
  );
}





function SearchBookModal({ onClose }) {
  const [title, setTitle] = useState('');
  const [author, setAuthor] = useState('');
  const [genre, setGenre] = useState('');
  const [results, setResults] = useState([]);
  const [error, setError] = useState('');

  const handleSearch = () => {
    const query = new URLSearchParams({ title, author, genre }).toString();

    fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/SearchBooks?${query}`)
      .then(res => {
        if (!res.ok) throw new Error('Search failed');
        return res.json();
      })
      .then(setResults)
      .catch(() => setError('Search failed.'));
  };

  return (
    <div style={modalStyle}>
      <h3>🔍 Search Books</h3>
      <label>Title</label><br />
      <input value={title} onChange={e => setTitle(e.target.value)} /><br />

      <label>Author</label><br />
      <input value={author} onChange={e => setAuthor(e.target.value)} /><br />

      <label>Genre</label><br />
      <input value={genre} onChange={e => setGenre(e.target.value)} /><br />

      <button onClick={handleSearch}>Search</button>
      <button onClick={onClose} style={{ marginLeft: '10px' }}>Close</button>

      {error && <p style={{ color: 'red' }}>{error}</p>}

      <ul>
        {results.map(book => (
          <li key={book.id}>
            {book.title} by {book.author} ({book.genre})
            {book.status && <em> — {book.status}</em>}
          </li>
        ))}
      </ul>
    </div>
  );
}






function App() {
  const [bookshelves, setBookshelves] = useState([]);
  const [showAddModal, setShowAddModal] = useState(false);
  const [showRemoveModal, setShowRemoveModal] = useState(false);
  const [showAddBookModal, setShowAddBookModal] = useState(false);
  const [showAutoPlaceBookModal, setShowAutoPlaceBookModal] = useState(false);
  const [newShelf, setNewShelf] = useState({ label: '', height: '', width: '', shelfHeight: '' });
  const [removeLabel, setRemoveLabel] = useState('');
  const [depotBooks, setDepotBooks] = useState([]);
  const [activeShelf, setActiveShelf] = useState(null);
  const [borrowedBooks, setBorrowedBooks] = useState([]);
  const [showQRModal, setShowQRModal] = useState(false);

  
  const [showSearchModal, setShowSearchModal] = useState(false);


  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = () => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/Bookshelves')
      .then(res => res.json())
      .then(setBookshelves)
      .catch(console.error);

    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/DepotBooks')
      .then(res => res.json())
      .then(setDepotBooks)
      .catch(console.error);

    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AllBooks')
      .then(res => res.json())
      .then(data => {
        const borrowed = data.filter(b => b.status?.toLowerCase() === 'borrowed');
        setBorrowedBooks(borrowed);
      })
      .catch(console.error);
  };


  const calculateUsage = (shelf) => {
    const used = shelf.books?.reduce((sum, b) => sum + b.width, 0) || 0;
    return (used / shelf.width) * 100;
  };

  const getColor = (percentage) => {
    const red = Math.min(255, Math.round((percentage / 100) * 255));
    const green = Math.max(0, 255 - red);
    return `rgb(${red},${green},0)`;
  };

  const handleAddShelf = () => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AddBookshelf', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ...newShelf,
        height: +newShelf.height,
        width: +newShelf.width,
        shelfHeight: +newShelf.shelfHeight
      })
    })
      .then(() => {
        setShowAddModal(false);
        fetchData();
      })
      .catch(console.error);
  };

  const handleRemoveShelf = () => {
    fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/RemoveBookshelf?label=${removeLabel}`, {
      method: 'DELETE'
    })
      .then(() => {
        setShowRemoveModal(false);
        fetchData();
      })
      .catch(console.error);
  };

  const handleShelfClick = (shelf, bookshelfLabel) => {
    setActiveShelf({ ...shelf, bookshelfLabel });
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>📚 Library Bookshelves</h1>

      <div style={{ marginBottom: '20px' }}>
        <button onClick={() => setShowAddModal(true)}>➕ Add Bookshelf</button>
        <button onClick={() => setShowRemoveModal(true)} style={{ marginLeft: '10px' }}>🗑️ Remove Bookshelf</button>
        <button onClick={() => setShowAddBookModal(true)} style={{ marginLeft: '10px' }}>📖 Add Book</button>
        <button onClick={() => setShowAutoPlaceBookModal(true)} style={{ marginLeft: '10px' }}>🤖 Auto Place Book</button>
        <button onClick={() => setShowSearchModal(true)} style={{ marginLeft: '10px' }}>🔍 Search Book</button>
        <button onClick={() => setShowQRModal(true)} style={{ marginLeft: '10px' }}>📷 Scan QR Code</button>

      </div>

      <div className="bookshelf-container">        {bookshelves.map((shelfGroup) => (
          <div key={shelfGroup.label} className="bookshelf-column">
            <h2>Bookshelf {shelfGroup.label}</h2>
            {shelfGroup.shelves?.map((shelf) => {
              const usage = calculateUsage(shelf);
              const color = getColor(usage);
              return (
                <div
                    key={shelf.position}
                    className="shelf-row"
                    onClick={() => handleShelfClick(shelf, shelfGroup.label)}
                  >
                    <div className="shelf-info">
                      <strong>Shelf {shelf.position}</strong> — {shelf.books?.length || 0} books — {usage.toFixed(1)}% full
                    </div>
                    <div className="shelf-bar">
                      <div
                        className="shelf-fill"
                        style={{ width: `${usage}%`, background: color }}
                      />
                    </div>
                  </div>

              );
            })}
          </div>
        ))}
      </div>

      <div style={{ marginTop: '40px' }}>
        <h2>🗃️ Depot</h2>
        <button onClick={() => {
          fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AutoPlaceDepotBooks', { method: 'POST' })
            .then(fetchData)
            .catch(console.error);
        }}>🛠️ Auto-Place All Depot Books</button>
        {depotBooks.length === 0 ? (
          <p>No books in depot.</p>
        ) : (
          <ul>
            {depotBooks.map(book => (
              <li key={book.id}>{book.title} by {book.author}</li>
            ))}
          </ul>
        )}
      </div>
      <div style={{ marginTop: '40px' }}>
          <h2>📕 Borrowed Books</h2>
          {borrowedBooks.length === 0 ? (
            <p>No borrowed books.</p>
          ) : (
            <ul>
              {borrowedBooks.map(book => (
                <li key={book.id}>
                  {book.title} by {book.author} (Borrowed by ID: {book.borrowerId})
                  <button
                    style={{ marginLeft: '10px' }}
                    onClick={() => {
                      fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/ReturnBook?title=${encodeURIComponent(book.title)}&borrowerId=${encodeURIComponent(book.borrowerId)}`, {
                        method: 'POST'
                      })
                      .then(fetchData)
                      .catch(console.error);
                    }}
                  >
                    ↩️ Return
                  </button>
                </li>
              ))}
            </ul>
          )}
        </div>


      {showAddBookModal && (
        <AddBookModal
          onClose={() => setShowAddBookModal(false)}
          onAdd={fetchData}
        />
      )}

      {showAutoPlaceBookModal && (
        <AddBookModal
          onClose={() => setShowAutoPlaceBookModal(false)}
          onAdd={fetchData}
          autoPlace={true}
        />
      )}
      
      {showSearchModal && (
          <SearchBookModal onClose={() => setShowSearchModal(false)} />
      )}


      {showAddModal && (
        <div style={modalStyle}>
          <h3>Add Bookshelf</h3>
          <input placeholder="Label" value={newShelf.label} onChange={e => setNewShelf({ ...newShelf, label: e.target.value })} /><br />
          <input placeholder="Height (cm)" value={newShelf.height} onChange={e => setNewShelf({ ...newShelf, height: e.target.value })} /><br />
          <input placeholder="Width (cm)" value={newShelf.width} onChange={e => setNewShelf({ ...newShelf, width: e.target.value })} /><br />
          <input placeholder="Shelf Height (cm)" value={newShelf.shelfHeight} onChange={e => setNewShelf({ ...newShelf, shelfHeight: e.target.value })} /><br />
          <button onClick={handleAddShelf}>Add</button>
          <button onClick={() => setShowAddModal(false)}>Cancel</button>
        </div>
      )}

      {showQRModal && (
        <QRScannerModal
          key="scanner" // ensures remount
          onClose={() => setShowQRModal(false)}
          onSuccess={fetchData}
        />
      )}




      {showRemoveModal && (
        <div style={modalStyle}>
          <h3>Remove Bookshelf</h3>
          <input placeholder="Label" value={removeLabel} onChange={e => setRemoveLabel(e.target.value)} /><br />
          <button onClick={handleRemoveShelf}>Remove</button>
          <button onClick={() => setShowRemoveModal(false)}>Cancel</button>
        </div>
      )}

      {activeShelf && (
          <div style={modalStyle}>
            <h3>Shelf {activeShelf.position} on Bookshelf {activeShelf.bookshelfLabel}</h3>
            {activeShelf.books?.length > 0 ? (
              <ul>
                {activeShelf.books.map(book => (
                  <li key={book.id}>
                    {book.title} by {book.author}
                    <div style={{ marginTop: '5px' }}>
                      <button onClick={() => {
                        fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/MoveBookByTitle?title=${book.title}`, {
                          method: 'POST'
                        }).then(() => {
                          setActiveShelf(null);
                          fetchData();
                        }).catch(console.error);
                      }}>📦 Move to Depot</button>

                      <button onClick={() => {
                        const borrowerId = prompt("Enter Borrower ID:");
                        if (borrowerId) {
                          fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/BorrowBook?title=${book.title}&borrowerId=${borrowerId}`, {
                            method: 'POST'
                          }).then(() => {
                            setActiveShelf(null);
                            fetchData();
                          }).catch(console.error);
                        }
                      }} style={{ marginLeft: '10px' }}>📚 Borrow</button>

                      <button onClick={() => {
                        const label = prompt("Enter target bookshelf label:");
                        const position = prompt("Enter target shelf position:");
                        if (label && position) {
                          fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/MoveBookByTitle?title=${book.title}&label=${label}&position=${position}`, {
                            method: 'POST'
                          }).then(() => {
                            setActiveShelf(null);
                            fetchData();
                          }).catch(console.error);
                        }
                      }} style={{ marginLeft: '10px' }}>📤 Move</button>

                      <button onClick={() => {
                        if (window.confirm(`Are you sure you want to remove "${book.title}"?`)) {
                          console.log("Trying to remove book with ID:", book.id, book);

                          fetch(`https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/RemoveBookById?id=${book.id}`, {
                            method: 'DELETE'
                          }).then(() => {
                            setActiveShelf(null);
                            fetchData();
                          }).catch(console.error);
                        }
                      }} style={{ marginLeft: '10px', color: 'red' }}>❌ Remove</button>
                    </div>
                  </li>
                ))}
              </ul>
            ) : (
              <p>No books on this shelf.</p>
            )}
            <button onClick={() => setActiveShelf(null)} style={{ marginTop: '10px' }}>Close</button>
          </div>
        )}

    </div>
  );
}

export default App;
