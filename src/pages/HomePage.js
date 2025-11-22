import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { Helmet } from 'react-helmet-async';

const HomePage = () => {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch('/articles.json')
      .then(res => {
        if (!res.ok) throw new Error('Failed to fetch articles');
        return res.json();
      })
      .then(data => {
        setArticles(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <div className="loading">Loading articles...</div>;
  if (error) return <div className="error">{error}</div>;

  const featuredArticle = articles.find(article => article.featured);
  const otherArticles = articles.filter(article => !article.featured);

  return (
    <>
      <Helmet>
        <title>Erewash Rag</title>
        <meta name="description" content="Satirical local news blog - bringing humor to your neighborhood" />
        <meta name="keywords" content="Erewash Rag, satirical news, local news, humor, blog, Erewash, UK, parody, entertainment" />
        <link rel="canonical" href="https://erewash-rag.co.uk/" />
        <meta property="og:type" content="website" />
        <meta property="og:title" content="Erewash Rag" />
        <meta property="og:description" content="Satirical local news blog - bringing humor to your neighborhood" />
        <meta property="og:url" content="https://erewash-rag.co.uk/" />
        <meta property="og:image" content="https://erewash-rag.co.uk/favicon.svg" />
        <meta name="twitter:card" content="summary_large_image" />
        <meta name="twitter:title" content="Erewash Rag" />
        <meta name="twitter:description" content="Satirical local news blog - bringing humor to your neighborhood" />
        <meta name="twitter:image" content="https://erewash-rag.co.uk/favicon.svg" />
      </Helmet>


      {/* Other Articles Grid */}
      <section className="articles-section">
        <div className="articles-grid">
          {otherArticles.map(article => (
            <ArticleCard key={article.id} article={article} />
          ))}
        </div>
      </section>
    </>
  );
};

const ArticleCard = ({ article }) => {
  return (
    <Link to={`/articles/${article.id}`} className="article-card">
      <img 
        src={article.image} 
        alt={article.title}
        className="article-image"
      />
      <div className="article-content">
        {/* <span className="article-category">{article.category}</span> */}
        <h3 className="article-title">{article.title}</h3>
        <p className="article-excerpt">{article.excerpt}</p>
      </div>
    </Link>
  );
};

export default HomePage; 