'use client';

import React, { useState, useEffect } from 'react';

const translations = {
    tr: {
        "footer-copyright": "© 2025 PCPartsBuild. Tüm Hakları Saklıdır. Bize Ulaşın:"
    },
    en: {
        "footer-copyright": "© 2025 PCPartsBuild. All Rights Reserved. Contact Us:"
    }
};

export default function Footer() {
    const [lang, setLang] = useState('tr');

    useEffect(() => {
        const savedLang = localStorage.getItem('lang') || 'tr';
        setLang(savedLang);
    }, []);

    const t = translations[lang];

    return (
        <footer className="flex flex-col items-center gap-6 py-10 text-center">
            <div className="flex flex-wrap justify-center gap-4">
                <a className="text-white/60 transition-colors hover:text-white" href="#">
                    <svg aria-hidden="true" className="h-6 w-6" fill="currentColor" viewBox="0 0 24 24"><path d="M8.29 20.251c7.547 0 11.675-6.253 11.675-11.675 0-.178 0-.355-.012-.53A8.348 8.348 0 0022 5.92a8.19 8.19 0 01-2.357.646 4.118 4.118 0 001.804-2.27 8.224 8.224 0 01-2.605.996 4.107 4.107 0 00-6.993 3.743 11.65 11.65 0 01-8.457-4.287 4.106 4.106 0 001.27 5.477A4.072 4.072 0 012.8 9.71v.052a4.105 4.105 0 003.292 4.022 4.095 4.095 0 01-1.853.07 4.108 4.108 0 003.834 2.85A8.233 8.233 0 012 18.407a11.616 11.616 0 006.29 1.84"></path></svg>
                </a>
                <a className="text-white/60 transition-colors hover:text-white" href="#">
                    <svg aria-hidden="true" className="h-6 w-6" fill="currentColor" viewBox="0 0 24 24"><path clipRule="evenodd" d="M22 12c0-5.523-4.477-10-10-10S2 6.477 2 12c0 4.991 3.657 9.128 8.438 9.878v-6.987h-2.54V12h2.54V9.797c0-2.506 1.492-3.89 3.777-3.89 1.094 0 2.238.195 2.238.195v2.46h-1.26c-1.243 0-1.63.771-1.63 1.562V12h2.773l-.443 2.89h-2.33v6.988C18.343 21.128 22 16.991 22 12z" fillRule="evenodd"></path></svg>
                </a>
                <a className="text-white/60 transition-colors hover:text-white" href="#">
                    <svg aria-hidden="true" className="h-6 w-6" fill="currentColor" viewBox="0 0 24 24"><path clipRule="evenodd" d="M12.315 2c2.43 0 2.784.013 3.808.06 1.064.049 1.791.218 2.427.465a4.902 4.902 0 011.772 1.153 4.902 4.902 0 011.153 1.772c.247.636.416 1.363.465 2.427.048 1.024.06 1.378.06 3.808s-.012 2.784-.06 3.808c-.049 1.064-.218 1.791-.465 2.427a4.902 4.902 0 01-1.153 1.772 4.902 4.902 0 01-1.772 1.153c-.636.247-1.363.416-2.427.465-1.024.048-1.378.06-3.808.06s-2.784-.013-3.808-.06c-1.064-.049-1.791-.218-2.427-.465a4.902 4.902 0 01-1.772-1.153 4.902 4.902 0 01-1.153-1.772c-.247-.636-.416-1.363-.465-2.427-.048-1.024-.06-1.378-.06-3.808s.012-2.784.06-3.808c.049-1.064.218-1.791.465-2.427a4.902 4.902 0 011.153-1.772A4.902 4.902 0 016.345 2.525c.636-.247 1.363-.416 2.427-.465C9.793 2.013 10.147 2 12.315 2zm0 1.623c-2.403 0-2.72.01-3.667.058-1.146.052-1.602.248-1.922.458-.426.28-.765.624-1.043 1.043-.28.426-.624.765-1.043 1.043-.21.32-.406.776-.458 1.922-.048.947-.058 1.264-.058 3.667s.01 2.72.058 3.667c.052 1.146.248 1.602.458 1.922.28.426.624.765 1.043 1.043.426.28.765.624 1.043 1.043.32.21.776.406 1.922.458.947.048 1.264.058 3.667.058s2.72-.01 3.667-.058c1.146-.052 1.602-.248 1.922-.458.426-.28.765-.624 1.043-1.043.28-.426.624-.765 1.043-1.043.32-.21.406-.776.458-1.922.048-.947.058-1.264.058-3.667s-.01-2.72-.058-3.667c-.052-1.146-.248-1.602-.458-1.922-.28-.426-.624-.765-1.043-1.043-.426-.28-.765-.624-1.043-1.043-.32-.21-.776-.406-1.922-.458-.947-.048-1.264-.058-3.667-.058zM12 6.865a5.135 5.135 0 100 10.27 5.135 5.135 0 000-10.27zm0 1.623a3.512 3.512 0 110 7.024 3.512 3.512 0 010-7.024zM16.536 6.808a1.2 1.2 0 100 2.4 1.2 1.2 0 000-2.4z" fillRule="evenodd"></path></svg>
                </a>
            </div>
            <p className="text-white/60 text-sm font-normal leading-normal" data-i18n="footer-copyright-container">
                <span data-i18n="footer-copyright">{t["footer-copyright"]}</span> <a className="underline hover:text-primary" href="mailto:pcpartsbuild@gmail.com">pcpartsbuild@gmail.com</a>
            </p>
        </footer>
    );
}
