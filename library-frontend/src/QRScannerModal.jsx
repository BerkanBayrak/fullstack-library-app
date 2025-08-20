import React, { useEffect, useRef, useState } from 'react';
import { Html5QrcodeScanner } from 'html5-qrcode';

export default function QRScannerModal({ onClose, onSuccess }) {
  const scannerRef = useRef(null); // persists between renders
  const [scannedBook, setScannedBook] = useState(null);
  const [bookshelves, setBookshelves] = useState([]);
  const [label, setLabel] = useState('');
  const [position, setPosition] = useState('');

  useEffect(() => {
    const timeout = setTimeout(() => {
      const container = document.getElementById('qr-reader');
      if (!container) return;

      container.innerHTML = ''; // Clean up in case

      const scanner = new Html5QrcodeScanner('qr-reader', { fps: 10, qrbox: 250 }, false);
      scannerRef.current = scanner;

      scanner.render(
        (text) => {
          try {
            const data = JSON.parse(text);
            setScannedBook(data);
          } catch {
            alert("Invalid QR code. Expecting JSON.");
          }

          scanner.clear().then(() => {
            scannerRef.current = null;
            if (container) container.innerHTML = '';
          });
        },
        (err) => {
          console.warn("QR Scan Error:", err);
        }
      );
    }, 100); // ⏱ small delay ensures DOM is ready

    return () => {
      clearTimeout(timeout); // cleanup timeout
      if (scannerRef.current) {
        scannerRef.current.clear().then(() => {
          const container = document.getElementById('qr-reader');
          if (container) container.innerHTML = '';
          scannerRef.current = null;
        });
      }
    };
  }, []);


  const handleClose = () => {
    setScannedBook(null); // 👈 resets scanner view
    onClose();
  };

  useEffect(() => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/Bookshelves')
      .then(res => res.json())
      .then(setBookshelves)
      .catch(console.error);
  }, []);

  const handleAddDepot = () => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AddBook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...scannedBook, label: "", position: null })
    }).then(() => {
      onSuccess();
      onClose();
    }).catch(console.error);
  };

  const handleAutoPlace = () => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AutoPlaceBook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(scannedBook)
    }).then(() => {
      onSuccess();
      onClose();
    }).catch(console.error);
  };

  const handleManualAdd = () => {
    fetch('https://cloudlibrary-backend-585774735601.us-central1.run.app/api/Library/AddBook', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ...scannedBook,
        label,
        position: position ? parseInt(position) : null
      })
    }).then(() => {
      onSuccess();
      onClose();
    }).catch(console.error);
  };

  return (
    <div style={{
      position: 'fixed',
      top: '20%',
      left: '30%',
      width: '500px',
      maxHeight: '80vh',
      backgroundColor: 'white',
      padding: '20px',
      boxShadow: '0 0 10px rgba(0,0,0,0.3)',
      zIndex: 1000,
      borderRadius: '10px'
    }}>
      <h3>📷 Scan Book QR</h3>

      {!scannedBook && <div id="qr-reader" style={{ width: '100%' }} />}

      {scannedBook && (
        <>
          <p><strong>Title:</strong> {scannedBook.title}</p>
          <p><strong>Author:</strong> {scannedBook.author}</p>
          <p><strong>Genre:</strong> {scannedBook.genre}</p>
          <p><strong>Height:</strong> {scannedBook.height} cm</p>
          <p><strong>Width:</strong> {scannedBook.width} cm</p>
          <p><strong>Type:</strong> {scannedBook.type}</p>

          <div style={{ marginTop: '10px' }}>
            <button onClick={handleAddDepot}>📦 Add to Depot</button>
            <button onClick={handleAutoPlace} style={{ marginLeft: '10px' }}>🤖 Auto Place Book</button>
          </div>

          <div style={{ marginTop: '20px' }}>
            <h4>📍 Manual Placement</h4>
            <label>Bookshelf Label:</label><br />
            <select value={label} onChange={e => setLabel(e.target.value)}>
              <option value="">Select</option>
              {bookshelves.map(bs => (
                <option key={bs.label} value={bs.label}>{bs.label}</option>
              ))}
            </select><br />

            <label>Shelf Position:</label><br />
            <input
              type="number"
              value={position}
              onChange={e => setPosition(e.target.value)}
            /><br />

            <button onClick={handleManualAdd} disabled={!label || !position}>➕ Add to Shelf</button>
          </div>
        </>
      )}

      <button onClick={handleClose} style={{ marginTop: '20px' }}>Close</button>
    </div>
  );
}
